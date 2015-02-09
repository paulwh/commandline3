using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine.Core;
using CommandLine.Helpers;
using CommandLine.Text;

namespace CommandLine {
    public class Parser {
        private ParserSettings settings;
        private bool hasShortFormPrefix;
        private char shortFormPrefix;
        private int longFormPrefixLength;

        public static Parser Default {
            get {
                return new Parser(new ParserSettings());
            }
        }

        public Parser(ParserSettings settings) {
            if (settings == null) {
                throw new ArgumentNullException("settings");
            }

            this.hasShortFormPrefix = settings.ShortOptionPrefix.HasValue;
            this.shortFormPrefix = settings.ShortOptionPrefix.Value;
            this.longFormPrefixLength = this.settings.LongOptionPrefix.Length;

            if (this.hasShortFormPrefix &&
                settings.LongOptionPrefix == this.shortFormPrefix.ToString()) {
                throw new ArgumentException(
                    "The sort option prefix and long option prefix cannot be the same.",
                    "settings"
                );
            }

            this.settings = settings;
        }

        public event EventHandler<GenerateHelpTextEventArgs> GenerateHelpText = delegate { };

        public ParserResult<T> ParseArguments<T>(string[] args) where T : new() {
            if (args == null) {
                throw new ArgumentNullException("args");
            }

            return ParseArguments(new T(), args);
        }

        public ParserResult<T> ParseArguments<T>(T instance, string[] args) {
            if (instance == null) {
                throw new ArgumentNullException("instance");
            } else if (args == null) {
                throw new ArgumentNullException("args");
            }

            return this.ParseArgumentsImpl(instance, args);
        }

        private ParserResult<T> ParseArgumentsImpl<T>(T instance, IEnumerable<string> args) {
            var optionLookup = OptionLookup.ForType<T>();
            var handlerResults = HandleTokens(
                optionLookup,
                this.Tokenize(args)
            ).ToList();

            var errors = new List<Error>();

            // Group the option values that are for the same option if the
            // option can accept multiple values.
            var groupedOptionValues = new List<OptionValue>();
            foreach (var result in handlerResults) {
                result.WithEither(
                    error => errors.Add(error),
                    option => {
                        var existingIx =
                            groupedOptionValues.FindIndex(existing => existing.Spec == option.Spec);
                        if (existingIx >= 0) {
                            if (option.Spec.Deserializer.AcceptsMultipleValues) {
                                groupedOptionValues[existingIx] =
                                    groupedOptionValues[existingIx].WithAdditionaValues(option.Values);
                            } else {
                                errors.Add(
                                    new DuplicateOptionError(
                                        option.Name,
                                        groupedOptionValues[existingIx].Values.SingleOrDefault()
                                    )
                                );
                                // in theory this option could just generate a
                                // warning so we proceed with adding the option
                                // the behavior will depend on the deserializer
                                groupedOptionValues.Add(option);
                            }
                        } else {
                            groupedOptionValues.Add(option);
                        }
                    }
                );
            }

            // check for mutually exclusive set violations
            var parameterSetOptions =
                groupedOptionValues
                    .Where(op => op.Spec.ParameterSetName != null)
                    .GroupBy(op => op.Spec.ParameterSetName, op => op.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList();

            if (parameterSetOptions.Count > 1) {
                errors.Add(
                    new MutuallyExclusiveSetError(parameterSetOptions)
                );
            }

            // check for missing required options
            var presentParameterSets =
                new HashSet<string>(parameterSetOptions.Select(grp => grp.Key), StringComparer.OrdinalIgnoreCase);
            var presentOptions =
                new HashSet<OptionSpec>(groupedOptionValues.Select(op => op.Spec));

            Func<OptionSpec, bool> isRequiredAndMissing =
                spec =>
                    // the parameter is required
                    spec.Required &&
                    // either it has not parameter set, or it's parameter set
                    // has been triggered
                    (String.IsNullOrEmpty(spec.ParameterSetName) ||
                        presentParameterSets.Contains(spec.ParameterSetName)) &&
                    // the option is not present
                    !presentOptions.Contains(spec);
            foreach (var missing in optionLookup.All.Where(isRequiredAndMissing)) {
                errors.Add(
                    new MissingRequiredOptionError(
                        missing.OptionName
                    )
                );
            }

            // process the options
            foreach (var option in groupedOptionValues) {
                var property = option.Spec.Property;
                var deserializer = option.Spec.Deserializer;
                var valueInstance =
                    property.GetValue(instance) ??
                        deserializer.CreateInstance();
                var values = option.Values.ToList();
                if (values.Any()) {
                    try {
                        if (deserializer.AcceptsMultipleValues) {
                            valueInstance =
                                deserializer.Deserialize(
                                    this.settings.ParsingCulture,
                                    valueInstance,
                                    values
                                );
                        } else {
                            valueInstance =
                                deserializer.Deserialize(
                                    this.settings.ParsingCulture,
                                    valueInstance,
                                    values.SingleOrDefault()
                                );
                        }

                        if (property.CanWrite) {
                            property.SetValue(instance, valueInstance);
                        }
                    } catch (DeserializationException ex) {
                        errors.Add(
                            new BadValueFormatError(option.Name, ex.Value, ex.Message)
                        );
                    }
                } else if (option.Spec.IsSwitch) {
                    if (property.CanWrite) {
                        property.SetValue(instance, true);
                    } else {
                        // this could be applied more broadly to any option
                        // property that is a struct type.
                        throw new InvalidOperationException(
                            "Unable to set the property for a switch option."
                        );
                    }
                } else {
                    errors.Add(
                        new MissingValueError(option.Name)
                    );
                }
            }

            var parserResult = new ParserResult<T>(
                instance,
                errors,
                optionLookup.All
            );

            var criticalErrors =
                this.settings.IgnoreUnknownArguments ?
                    errors.Where(e => e.Type != ErrorType.UnknownOptionError && e.Type != ErrorType.UnexpectedValueError) :
                    errors;
            if (this.settings.AutomaticHelpOutput && criticalErrors.Any()) {

                var generateHelpTextArg =
                    new GenerateHelpTextEventArgs { HelpText = HelpText.AutoBuild(this.settings, parserResult) };
                this.GenerateHelpText(this, generateHelpTextArg);
                if (generateHelpTextArg.HelpText != null) {
                    generateHelpTextArg.HelpText.Render(this.settings.HelpWriter);
                }
            }

            return parserResult;
        }


        /// <summary>
        /// This method turns a stream of tokens (either options or values)
        /// into either OptionValue pairs that link each option with zero or
        /// more option, or errors representing unhandled or missing tokens.
        /// </summary>
        internal IEnumerable<Either<Error, OptionValue>> HandleTokens(OptionLookup optionSpecs, IEnumerable<Token> tokens) {
            OptionSpec current = null;
            var values = new Queue<string>();
            // the preceding option had the format --foo=
            bool valueExpected = false;
            int position = -1;
            foreach (var token in tokens) {
                switch (token.Type) {
                    case TokenType.Option:
                    case TokenType.OptionExpectingValue:
                    case TokenType.ShortOption:
                        var name =
                            token.Type == TokenType.ShortOption ?
                                new OptionName(null, token.Value[0]) :
                                new OptionName(token.Value, null);

                        if (current != null) {
                            // yield the value for the previous option
                            if (valueExpected) {
                                // this should never happen, since --foo= would
                                // be treated as having the value String.Empty
                                yield return (Error)new MissingValueError(current.OptionName);
                                valueExpected = false;
                            }
                            yield return new OptionValue(
                                current,
                                name,
                                values
                            );
                            current = null;
                            values = new Queue<string>();
                        }

                        // detect option for the current token
                        current = optionSpecs.ForToken(token);
                        if (current == null) {
                            if (IsHelpToken(token)) {
                                yield return HelpRequestedError.Instance;
                            } else {
                                yield return new UnknownOptionError(name);
                            }
                        } else {
                            valueExpected = (token.Type == TokenType.OptionExpectingValue);
                        }
                        break;
                    case TokenType.Value:
                        // determine whether this value can be mapped to an
                        // existing option, or can be mapped to a
                        // positional parameter.
                        if (current != null) {
                            values.Enqueue(token.Value);
                            if (valueExpected || !IsListOption(current)) {
                                // yield the current option with just the
                                // associated value.
                                yield return new OptionValue(
                                    current,
                                    current.OptionName,
                                    values
                                );
                                values = new Queue<string>();
                                valueExpected = false;
                                current = null;
                            } // otherwise, continue accumulating values
                        } else if (++position < optionSpecs.ByPosition.Count) {
                            // there's a positional parameter we can bind this
                            // value to
                            current = optionSpecs.ByPosition[position];
                            values.Enqueue(token.Value);
                            // if this isn't the last positional parameter, or
                            // it isn't a list option, yield it immediately
                            // (otherwise we accumulate more values)
                            if (position + 1 < optionSpecs.ByPosition.Count ||
                                !IsListOption(current)) {
                                yield return new OptionValue(
                                    current,
                                    current.OptionName,
                                    values
                                );
                                values = new Queue<string>();
                                current = null;
                            }
                        } else {
                            // this value doesn't to correspond to any option
                            yield return (Error)new UnexpectedValueError(token.Value);
                        }
                        break;
                }
            }
            if (current != null) {
                yield return new OptionValue(
                    current,
                    current.OptionName,
                    values
                );
            }
        }

        private static readonly string[] HelpTokens =
            new[] { "help", "h", "?" };

        private bool IsHelpToken(Token token) {
            return HelpTokens.Contains(token.Value, this.settings.StringComparer);
        }

        private bool IsListOption(OptionSpec option) {
            return option.Deserializer.AcceptsMultipleValues;
        }

        public ParserResult<T> ParseMultiVerbArguments<T>(string[] args) where T : new() {
            return this.ParseMultiVerbArguments(new T(), args);
        }

        public ParserResult<T> ParseMultiVerbArguments<T>(T instance, string[] args) {
            if (instance == null) {
                throw new ArgumentNullException("instance");
            } else if (args == null) {
                throw new ArgumentNullException("args");
            }

            var verbProperties =
                typeof(T)
                    .GetProperties()
                    .Select(pi => new {
                        Property = pi,
                        Type = pi.PropertyType,
                        Attribute = pi.PropertyType.GetCustomAttribute<VerbAttribute>(inherit: true)
                    })
                    .Where(p => p.Attribute != null);

            var result = this.ParseMultiVerbArgumentsImpl(
                args,
                verbProperties
                    .Select(vp =>
                        new VerbSpec(
                            vp.Attribute.Name,
                            vp.Attribute.HelpText,
                            vp.Type,
                            () => vp.Property.GetValue(instance) ??
                                Activator.CreateInstance(vp.Type, nonPublic: true)
                        ))
                    .ToList()
            );

            if (!String.IsNullOrEmpty(result.Verb) && result.Value != null) {
                // attempt to assign the verb type instance back to the parent instance if possible
                var verbProperty =
                    verbProperties.Single(vp =>
                        vp.Attribute.Name.Equals(
                            result.Verb,
                            this.settings.StringComparison
                        )
                    );

                if (verbProperty.Property.CanWrite) {
                    verbProperty.Property.SetValue(instance, result.Value, new object[0]);
                }
            }

            return new ParserResult<T>(
                instance,
                result.Errors,
                result.Options,
                result.VerbTypes,
                result.Verb
            );
        }

        public ParserResult<object> ParseMultiVerbArguments(string[] args, params Object[] verbInstances) {
            if (verbInstances.Length == 0) {
                throw new ArgumentException(
                    "One or more verb instances must be specified.",
                    "verbInstances"
                );
            } else if (verbInstances.Any(v => v == null)) {
                throw new ArgumentException(
                    "Verb instances cannot be null.",
                    "verbInstances"
                );
            }

            var verbInstanceDetails =
                verbInstances
                    .Select(vi => new {
                        Instance = vi,
                        Type = vi.GetType(),
                        Attribute = vi.GetType().GetCustomAttribute<VerbAttribute>(inherit: true)
                    })
                    .ToList();

            if (verbInstanceDetails.Any(vi => vi.Attribute == null)) {
                throw new ArgumentException(
                    "Invalid verb instance (type missing [Verb] attribute): " +
                        verbInstanceDetails.First(vi => vi.Attribute == null).Type.FullName,
                    "verbInstances"
                );
            }

            return ParseMultiVerbArgumentsImpl(
                args,
                verbInstanceDetails
                    .Select(vi =>
                        new VerbSpec(
                            vi.Attribute.Name,
                            vi.Attribute.HelpText,
                            vi.Type,
                            () => vi.Instance))
                    .ToList()
            );
        }

        public ParserResult<object> ParseMultiVerbArguments(string[] args, params Type[] verbTypes) {
            if (verbTypes.Length == 0) {
                throw new ArgumentException(
                    "One or more verb types must be specified.",
                    "verbTypes"
                );
            } else if (verbTypes.Any(t => t == null)) {
                throw new ArgumentException(
                    "Verb types cannot be null.",
                    "verbTypes"
                );
            }

            var verbTypeDetails =
                verbTypes
                    .Select(vt => new {
                        Type = vt,
                        Attribute = vt.GetCustomAttribute<VerbAttribute>(inherit: true)
                    })
                    .ToList();

            if (verbTypeDetails.Any(vt => vt.Attribute == null)) {
                throw new ArgumentException(
                    "Invalid verb type (missing [Verb] attribute): " +
                        verbTypeDetails.First(vt => vt.Attribute == null).Type.FullName,
                    "verbTypes"
                );
            }

            return ParseMultiVerbArgumentsImpl(
                args,
                verbTypeDetails
                    .Select(vt =>
                        new VerbSpec(
                            vt.Attribute.Name,
                            vt.Attribute.HelpText,
                            vt.Type,
                            () => Activator.CreateInstance(vt.Type, nonPublic: true)))
                    .ToList()
            );
        }

        private const string HelpVerb = "help";

        private ParserResult<object> ParseMultiVerbArgumentsImpl(string[] args, IList<VerbSpec> verbs) {
            if (args == null) {
                throw new ArgumentNullException("args");
            } else if (args.Length == 0) {
                return new ParserResult<object>(
                    null,
                    new[] { NoVerbSelectedError.Instance },
                    verbs
                );
            }

            var verbLookup =
                new Dictionary<string, VerbSpec>(this.settings.StringComparer);

            // build lookup table for verbs
            foreach (var verb in verbs) {
                var verbType = verb.VerbType;
                var verbAttribute = verbType.GetCustomAttribute<VerbAttribute>(inherit: true);
                if (verbAttribute == null) {
                    throw new ArgumentException(
                        "The specified verb type: " + verbType.FullName + " does not have a [Verb] attribute.",
                        "verbTypes"
                    );
                } else if (verbLookup.ContainsKey(verbAttribute.Name)) {
                    throw new ArgumentException(
                        "Multiple verb types had the same key: " + verbAttribute.Name,
                        "verbTypes"
                    );
                }

                verbLookup.Add(verbAttribute.Name, verb);
            }

            // Determine verb, skip, and call normal ParseVerbArguments
            VerbSpec selectedVerb;
            if (verbLookup.TryGetValue(args[0], out selectedVerb)) {
                var result =
                    (ParserResult<object>)ParseVerbArgumentsMethod
                        .MakeGenericMethod(selectedVerb.VerbType)
                        .Invoke(this, new object[] { selectedVerb.CreateInstance(), args.Skip(1) });

                return result.WithVerbInfo(verbs, args[0]);
            } else {
                ParserResult<object> result;
                if (HelpVerb.Equals(args[0], this.settings.StringComparison)) {
                    result = new ParserResult<object>(
                        null,
                        new[] { HelpVerbRequestedError.Instance },
                        verbs,
                        args[0]
                    );
                } else {
                    result = new ParserResult<object>(
                        null,
                        new[] { new BadVerbSelectedError(args[0]) },
                        verbs,
                        args[0]
                    );
                }

                if (this.settings.AutomaticHelpOutput) {
                    var generateHelpTextArg =
                        new GenerateHelpTextEventArgs { HelpText = HelpText.AutoBuild(this.settings, result) };
                    this.GenerateHelpText(this, generateHelpTextArg);
                    if (generateHelpTextArg.HelpText != null) {
                        generateHelpTextArg.HelpText.Render(this.settings.HelpWriter);
                    }
                }

                return result;
            }
        }

        private MethodInfo ParseVerbArgumentsMethod =
            ReflectionHelper.GetGenericMethodDefinition<Parser>(p => p.ParseVerbArguments<object>(null, null));

        private ParserResult<object> ParseVerbArguments<T>(T instance, IEnumerable<string> args) {
            return ParseArgumentsImpl<T>(instance, args).ToUntyped();
        }

        private IEnumerable<Token> Tokenize(IEnumerable<string> args) {
            foreach (var arg in args) {
                if (arg.StartsWith(this.settings.LongOptionPrefix)) {
                    var eqIx = arg.IndexOf('=');
                    if (eqIx >= 0) {
                        yield return new Token(
                            TokenType.OptionExpectingValue,
                            arg,
                            arg.Substring(this.longFormPrefixLength, eqIx)
                        );

                        yield return new Token(
                            TokenType.Value,
                            arg,
                            arg.Substring(eqIx + 1)
                        );
                    } else {
                        yield return new Token(
                            TokenType.Option,
                            arg,
                            arg.Substring(this.longFormPrefixLength)
                        );
                    }
                } else if (this.hasShortFormPrefix && arg.StartsWith(this.shortFormPrefix.ToString())) {
                    foreach (var c in arg.Skip(1)) {
                        yield return new Token(TokenType.ShortOption, arg, c.ToString());
                    }
                } else {
                    yield return new Token(TokenType.Value, arg, arg);
                }
            }
        }
    }
}
