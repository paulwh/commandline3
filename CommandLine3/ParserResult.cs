using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommandLine.Core;

namespace CommandLine {
    public class ParserResult<T> {
        public T Value { get; private set; }
        public IList<Error> Errors { get; private set; }
        public string Verb { get; private set; }
        internal IList<VerbSpec> VerbTypes { get; private set; }
        internal IList<OptionSpec> Options { get; private set; }

        internal ParserResult(T value)
            : this(value, Enumerable.Empty<Error>(), null, null) {
        }

        internal ParserResult(T value, IEnumerable<Error> errors, IList<OptionSpec> options)
            : this(value, errors, options, null, null) {
        }

        internal ParserResult(T value, IEnumerable<Error> errors, IList<VerbSpec> verbTypes)
            : this(value, errors, null, verbTypes, null) {
        }

        internal ParserResult(T value, IEnumerable<Error> errors, IList<VerbSpec> verbTypes, string verb)
            : this(value, errors, null, verbTypes, verb) {
        }

        internal ParserResult(T value, IEnumerable<Error> errors, IList<OptionSpec> options, IList<VerbSpec> verbTypes, string verb) {
            if (errors == null) {
                throw new ArgumentNullException("errors");
            }

            this.Value = value;
            this.Errors = new ReadOnlyCollection<Error>(errors.ToList());
            this.Options = options;
            this.VerbTypes = verbTypes;
            this.Verb = verb;
        }

        internal ParserResult<object> ToUntyped() {
            return new ParserResult<object>(
                (object)this.Value,
                this.Errors,
                this.Options,
                this.VerbTypes,
                this.Verb
            );
        }

        public ParserResult<T> WithVerbInfo(IList<VerbSpec> verbTypes, string verb) {
            return new ParserResult<T>(
                this.Value,
                this.Errors,
                this.Options,
                verbTypes,
                verb
            );
        }
    }
}
