using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CommandLine {
    public class ParserResult<T> {
        public T Value { get; private set; }
        public IList<Error> Errors { get; private set; }
        public string Verb { get; private set; }
        internal IEnumerable<Type> VerbTypes { get; private set; }

        internal ParserResult(T value, IEnumerable<Error> errors, IEnumerable<Type> verbTypes) {
            this.Value = value;
            this.Errors = new ReadOnlyCollection<Error>(errors.ToList());
            this.VerbTypes = verbTypes;
        }

        internal ParserResult(T value, IEnumerable<Error> errors, IEnumerable<Type> verbTypes, string verb)
            : this(value, errors, verbTypes) {

            this.Verb = verb;
        }
    }
}
