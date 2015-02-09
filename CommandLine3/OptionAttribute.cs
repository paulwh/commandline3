using System;

namespace CommandLine {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OptionAttribute : Attribute {
        public string LongName { get; private set; }
        public char? ShortName { get; private set; }
        public int? Position { get; private set; }
        public bool Required { get; set; }
        public string ParameterSetName { get; set; }
        public object DefaultValue { get; set; }
        public string HelpText { get; set; }
        public Type Validator { get; set; }
        public object[] ValidatorArguments { get; set; }
        public Type Deserializer { get; set; }
        public object[] DeserializerArguments { get; set; }

        public OptionAttribute() {
        }

        public OptionAttribute(char shortName, string longName) {
            this.ShortName = shortName;
            this.LongName = longName;
        }

        public OptionAttribute(string longName) {
            this.LongName = longName;
        }

        public OptionAttribute(char shortName) {
            this.ShortName = shortName;
        }

        public OptionAttribute(char shortName, string longName, int position) {
            this.ShortName = shortName;
            this.LongName = longName;
            this.Position = position;
        }

        public OptionAttribute(string longName, int position) {
            this.LongName = longName;
            this.Position = position;
        }

        public OptionAttribute(char? shortName, int position) {
            this.ShortName = shortName;
            this.Position = position;
        }
    }
}
