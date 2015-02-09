using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine.Helpers;

namespace CommandLine.Core {
    internal class OptionLookup {
        public IList<OptionSpec> All { get; private set; }
        public IDictionary<string, OptionSpec> ByLongName { get; private set; }
        public IDictionary<char, OptionSpec> ByShortName { get; private set; }
        public IDictionary<Maybe<string>, IList<OptionSpec>> ByPosition { get; private set; }

        public OptionLookup(
            IList<OptionSpec> all,
            IDictionary<string, OptionSpec> byLongName,
            IDictionary<char, OptionSpec> byShortName,
            IDictionary<Maybe<string>, IList<OptionSpec>> byPosition) {

            this.All = all;
            this.ByLongName = byLongName;
            this.ByShortName = byShortName;
            this.ByPosition = byPosition;
        }

        public OptionSpec GetOptionForToken(Token token) {
            switch (token.Type) {
                case TokenType.Option:
                case TokenType.OptionExpectingValue:
                    OptionSpec spec;
                    return this.ByLongName.TryGetValue(token.Value, out spec) ?
                        spec :
                        null;
                case TokenType.ShortOption:
                    return this.ByShortName.TryGetValue(token.Value[0], out spec) ?
                        spec :
                        null;
                default:
                    throw new NotSupportedException("TokenType " + token.Type + " not supported.");
            }
        }

        public static OptionLookup ForType(Type type) {
            return (OptionLookup)ForTypeMethod.MakeGenericMethod(type).Invoke(null, new object[0]);
        }

        private static readonly MethodInfo ForTypeMethod =
            ReflectionHelper.GetGenericMethodDefinition(() => ForType<object>());
        public static OptionLookup ForType<T>() {
            var optionSpecs =
                typeof(T)
                    .GetProperties()
                    .Select(pi => new OptionSpec(pi, pi.GetCustomAttribute<OptionAttribute>(inherit: false)))
                    .ToList();

            var longNameIndex =
                optionSpecs.ToDictionary(os => os.LongName);

            var shortNameIndex =
                optionSpecs
                    .Where(os => os.ShortName.HasValue)
                    .ToDictionary(os => os.ShortName.Value);

            var byParameterSet =
                optionSpecs
                    .GroupBy(os => (Maybe<string>)os.ParameterSetName)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.ToList());

            if (!byParameterSet.ContainsKey(null)) {
                byParameterSet[null] = new List<OptionSpec>();
            }

            // the byPosition lists are built per-paramter-set since positions
            // may be reused between different parameter sets
            var byPosition =
                byParameterSet
                    .Keys
                    .Select(ps => new {
                        ParameterSet = ps,
                        Options =
                            ps.HasValue ?
                                byParameterSet[null].Concat(byParameterSet[ps]) :
                                byParameterSet[null] })
                    .ToDictionary(
                        ps => ps.ParameterSet,
                        ps => ps.Options
                            .Where(os => os.Position.HasValue)
                            .GroupBy(os => os.Position.Value)
                            .Select(
                                grp => {
                                    try {
                                        return grp.Single();
                                    } catch {
                                        throw new InvalidOperationException(
                                            "Multiple options have the same position: " + String.Join(",", grp.Select(os => os.LongName))
                                        );
                                    }
                                })
                            .OrderBy(os => os.Position.Value)
                            .ToList() as IList<OptionSpec>
                    );

            return new OptionLookup(
                optionSpecs,
                longNameIndex,
                shortNameIndex,
                byPosition
            );
        }
    }
}
