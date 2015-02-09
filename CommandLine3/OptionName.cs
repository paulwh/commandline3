using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine {
    public class OptionName {
        public string LongName { get; private set; }

        public char? ShortName { get; private set; }

        public OptionName(string longName, char? shortName) {
            this.LongName = longName;
            this.ShortName = shortName;
        }

        internal string ToString(ParserSettings settings) {
            var builder = new StringBuilder();
            var hasLongForm = !String.IsNullOrEmpty(this.LongName) && !String.IsNullOrEmpty(settings.LongOptionPrefix);
            var hasShortForm = this.ShortName.HasValue && settings.ShortOptionPrefix.HasValue;
            if (hasShortForm) {
                builder.Append(settings.ShortOptionPrefix.Value);
                builder.Append(this.ShortName.Value);

                if (hasLongForm) {
                    builder.Append(settings.LongOptionPrefix.StartsWith("/") ? '|' : '/');
                }
            }
            if (hasLongForm) {
                builder.Append(settings.LongOptionPrefix);
                builder.Append(this.LongName);
            }
            return builder.ToString();
        }

        internal string ToShortString(ParserSettings settings) {
            var builder = new StringBuilder();
            var hasShortForm = this.ShortName.HasValue && settings.ShortOptionPrefix.HasValue;
            if (hasShortForm) {
                builder.Append(settings.ShortOptionPrefix.Value);
                builder.Append(this.ShortName.Value);
            } else {
                builder.Append(settings.LongOptionPrefix);
                builder.Append(this.LongName);
            }
            return builder.ToString();
        }
    }
}
