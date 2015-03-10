using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine.Core {
    internal class OptionSpec {
        public string LongName { get; private set; }
        public char? ShortName { get; private set; }
        public bool Required { get; private set; }
        public string ParameterSetName { get; private set; }
        public object DefaultValue { get; private set; }
        public string HelpText { get; private set; }
        public int? Position { get; private set; }
        public PropertyInfo Property { get; private set; }

        private Lazy<IOptionValueDeserializer> deserializer;
        public IOptionValueDeserializer Deserializer {
            get { return this.deserializer.Value; }
        }

        public OptionName OptionName {
            get { return new OptionName(this.LongName, this.ShortName); }
        }

        public bool IsSwitch {
            get {
                return this.Property.PropertyType == typeof(bool) ||
                    this.Property.PropertyType == typeof(bool?);
            }
        }

        public OptionSpec(PropertyInfo property, OptionAttribute attribute = null) {
            this.Property = property;

            if (attribute != null) {
                this.LongName = attribute.LongName;
                this.ShortName = attribute.ShortName;
                this.Required = attribute.Required;
                this.ParameterSetName = attribute.ParameterSetName;
                this.DefaultValue = attribute.DefaultValue;
                this.HelpText = attribute.HelpText;
                this.Position = attribute.Position;
            }

            if (this.LongName == null) {
                this.LongName = property.Name;
            }

            if (this.deserializer == null) {
                this.deserializer = new Lazy<IOptionValueDeserializer>(
                    () => OptionValueDeserializer.ForType(property.PropertyType)
                );
            }
        }

        public OptionSpec(String longName, char shortName, string helpText) {
            this.LongName = longName;
            this.ShortName = shortName;
            this.HelpText = helpText;
        }
    }
}
