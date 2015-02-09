using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine.Helpers;

namespace CommandLine.Text {
    public class ProgramCopyrightSection : HelpTextSection {
        public string Copyright { get; set; }

        protected override IEnumerable<string> RenderLines() {
            if (!String.IsNullOrEmpty(this.Copyright)) {
                yield return this.Copyright;
            }
        }

        internal static ProgramCopyrightSection AutoBuild() {
            return Assembly.GetEntryAssembly()
                .Bind(a => a.GetCustomAttribute<AssemblyCopyrightAttribute>())
                .Bind(attr => attr.Copyright.EmptyAsNull())
                .Bind(cr => new ProgramCopyrightSection { Copyright = cr });
        }
    }
}
