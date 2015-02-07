using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OptionAttribute {
        public string LongName { get; private set; }
        public char? ShortName { get; private set; }
        public string ParameterSetName { get; set; }
        public object DefaultValue { get; set; }
        public string HelpText { get; set; }
        public int? Position { get; set; }
        public Type Validator { get; set; }
        public object[] ValidatorArguments { get; set; }

        public OptionAttribute() {
        }

        public OptionAttribute(string longName, char? shortName) {
            this.LongName = longName;
            this.ShortName = shortName;
        }

        public OptionAttribute(string longName) {
            this.LongName = longName;
        }

        public OptionAttribute(char? shortName) {
            this.ShortName = shortName;
        }
    }
}
