using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CommandLine.Text {
    public class SelectedCommandSection : HelpTextSection {
        public string Verb { get; set; }
        public string HelpText { get; set; }
        public string FormatString { get; set; }

        public SelectedCommandSection() {
            this.FormatString = "Command: {0} - {1}";
        }

        protected override IEnumerable<string> RenderLines() {
            if (!String.IsNullOrEmpty(this.FormatString)) {
                yield return String.Format(
                    CultureInfo.InvariantCulture,
                    this.FormatString,
                    this.Verb,
                    this.HelpText
                );
            }
        }

        internal static SelectedCommandSection AutoBuild<T>(ParserSettings settings, ParserResult<T> result) {
            if (!String.IsNullOrEmpty(result.Verb)) {
                return new SelectedCommandSection {
                    Verb = result.Verb,
                    HelpText = result.VerbSpec.HelpText,
                    FormatString = settings.HelpTextResourceManager.GetString("SelectedVerbFormat")
                };
            } else {
                return null;
            }
        }
    }
}
