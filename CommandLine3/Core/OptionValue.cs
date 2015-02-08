using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Core {
    internal class OptionValue {
        public OptionSpec Spec { get; private set; }
        public OptionName Name { get; private set; }
        public IEnumerable<string> Values { get; private set; }

        public OptionValue(OptionSpec spec, OptionName name, IEnumerable<string> values) {
            this.Spec = spec;
            this.Name = name;
            this.Values = values;
        }

        internal OptionValue WithAdditionaValues(IEnumerable<string> values) {
            return new OptionValue(this.Spec, this.Name, this.Values.Concat(values));
        }
    }
}
