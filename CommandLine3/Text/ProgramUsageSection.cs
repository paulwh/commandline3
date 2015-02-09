using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using CommandLine.Core;

namespace CommandLine.Text {
    public class ProgramUsageSection : HelpTextSection {
        public string ExecutableName { get; set; }
        public List<string> ArgumentUsages { get; set; }
        public string FormatString { get; set; }

        public ProgramUsageSection(params string[] usages) {
            this.FormatString = "Usage: {0} {1}";
            this.ArgumentUsages = new List<string>(usages);
        }

        protected override IEnumerable<string> RenderLines() {
            if (!String.IsNullOrEmpty(this.FormatString)) {
                foreach (var usage in this.ArgumentUsages) {
                    yield return String.Format(
                        CultureInfo.InvariantCulture,
                        this.FormatString,
                        this.ExecutableName,
                        usage
                    );
                }
            }
        }

        internal static ProgramUsageSection AutoBuild(ParserSettings settings, IList<OptionSpec> options) {
            var parameterSets =
                new HashSet<string>(options.Select(os => os.ParameterSetName).Where(ps => !String.IsNullOrEmpty(ps)));

            List<string> usages;
            if (parameterSets.Any()) {
                usages = new List<string>();
                foreach (var parameterSet in parameterSets) {
                    var parameterSetOptions =
                        options.Where(os =>
                            String.IsNullOrEmpty(os.ParameterSetName) ||
                            os.ParameterSetName.Equals(parameterSet, StringComparison.OrdinalIgnoreCase));
                    usages.Add(BuildUsageString(settings, parameterSetOptions));
                }
            } else {
                usages = new List<string>(new[] { BuildUsageString(settings, options) });
            }

            return new ProgramUsageSection(usages.ToArray()) {
                ExecutableName = Process.GetCurrentProcess().ProcessName,
                FormatString = settings.HelpTextResourceManager.GetString("UsageStringFormat"),
                SubIndent = "    "
            };
        }

        internal static ProgramUsageSection AutoBuild(ParserSettings settings, IList<VerbSpec> verbs) {
            return new ProgramUsageSection("(" + String.Join("|", verbs.Select(vs => vs.VerbName)) + ")") {
                ExecutableName = Process.GetCurrentProcess().ProcessName,
                FormatString = settings.HelpTextResourceManager.GetString("UsageStringFormat"),
                SubIndent = "    "
            };
        }

        private static string BuildUsageString(ParserSettings settings, IEnumerable<OptionSpec> options) {
            var builder = new StringBuilder();
            var first = true;
            foreach (var option in options.OrderBy(os => os.Position)) {
                if (first) {
                    first = false;
                } else {
                    builder.Append(' ');
                }

                if (!option.Required) {
                    builder.Append('[');
                }
                if (option.Position.HasValue && !option.IsSwitch) {
                    builder.Append('[');
                }
                builder.Append(option.OptionName.ToString(settings));
                if (option.Position.HasValue && !option.IsSwitch) {
                    builder.Append(']');
                }
                if (!option.IsSwitch) {
                    if (option.Deserializer.AcceptsMultipleValues) {
                        builder.Append(" value1 value2 ...");
                    } else {
                        builder.Append(" value");
                    }
                }
                if (!option.Required) {
                    builder.Append(']');
                }

            }
            return builder.ToString();
        }
    }
}
