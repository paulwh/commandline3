using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommandLine.Core;
using CommandLine.Helpers;

namespace CommandLine {
    public class ParserResult<T> {
        public T Value { get; private set; }
        public IList<Error> Errors { get; private set; }
        public string Verb {
            get {
                return this.VerbSpec.Bind(vs => vs.VerbName);
            }
        }

        public string ParameterSet { get; private set; }
        internal VerbSpec VerbSpec { get; private set; }
        internal IList<VerbSpec> VerbTypes { get; private set; }
        internal IList<OptionSpec> Options { get; private set; }

        internal ParserResult(T value)
            : this(value, Enumerable.Empty<Error>(), null, null, null, null) {
        }

        internal ParserResult(T value, IEnumerable<Error> errors, IList<OptionSpec> options, string parameterSet = null)
            : this(value, errors, options, null, null, parameterSet) {
        }

        internal ParserResult(T value, IEnumerable<Error> errors, IList<VerbSpec> verbTypes)
            : this(value, errors, null, verbTypes, null, null) {
        }

        internal ParserResult(T value, IEnumerable<Error> errors, IList<VerbSpec> verbTypes, VerbSpec verb)
            : this(value, errors, null, verbTypes, verb, null) {
        }

        internal ParserResult(T value, IEnumerable<Error> errors, IList<OptionSpec> options, IList<VerbSpec> verbTypes, VerbSpec verb, string parameterSet) {
            if (errors == null) {
                throw new ArgumentNullException("errors");
            }

            this.Value = value;
            this.Errors = new ReadOnlyCollection<Error>(errors.ToList());
            this.Options = options;
            this.VerbTypes = verbTypes;
            this.VerbSpec = verb;
        }

        internal ParserResult<object> ToUntyped() {
            return new ParserResult<object>(
                (object)this.Value,
                this.Errors,
                this.Options,
                this.VerbTypes,
                this.VerbSpec,
                this.ParameterSet
            );
        }

        internal ParserResult<T> WithVerbInfo(IList<VerbSpec> verbTypes, VerbSpec verb) {
            return new ParserResult<T>(
                this.Value,
                this.Errors,
                this.Options,
                verbTypes,
                verb,
                this.ParameterSet
            );
        }

        private static readonly ErrorType[] VerbHelpErrorTypes =
            new[] { ErrorType.HelpVerbRequestedError, ErrorType.BadVerbSelectedError, ErrorType.NoVerbSelectedError };

        public bool IsVerbErrorResult() {
            return this.Errors.Any(e => VerbHelpErrorTypes.Contains(e.Type));
        }
    }
}
