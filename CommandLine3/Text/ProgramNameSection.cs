using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using CommandLine.Helpers;

namespace CommandLine.Text {
    public class ProgramNameSection : HelpTextSection {
        public string ProgramName { get; set; }
        public Version ProgramVersion { get; set; }
        public string FormatString { get; set; }

        public ProgramNameSection() {
            this.FormatString = "{0} {1}";
        }

        protected override IEnumerable<string> RenderLines() {
            if (!String.IsNullOrEmpty(this.FormatString)) {
                yield return String.Format(
                    CultureInfo.InvariantCulture,
                    this.FormatString,
                    this.ProgramName,
                    this.ProgramVersion
                );
            }
        }

        internal static ProgramNameSection AutoBuild(ParserSettings settings) {
            var assembly = Assembly.GetEntryAssembly();

            if (assembly != null) {
                var programName =
                    assembly.GetCustomAttribute<AssemblyTitleAttribute>().Bind(attr => attr.Title);
                return new ProgramNameSection {
                    ProgramName = programName ?? assembly.GetName().Name,
                    ProgramVersion = assembly.GetName().Version,
                    FormatString = settings.HelpTextResourceManager.GetString("AppTitleFormat")
                };
            } else {
                return null;
            }
        }
    }
}
