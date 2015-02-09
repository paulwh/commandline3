using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine.Helpers;

namespace CommandLine.Text {
    public class ProgramDescriptionSection : HelpTextSection {
        public string Description { get; set; }

        protected override IEnumerable<string> RenderLines() {
            if (!String.IsNullOrEmpty(this.Description)) {
                yield return this.Description;
            }
        }

        internal static ProgramDescriptionSection AutoBuild() {
            return Assembly.GetEntryAssembly()
                .Bind(a => a.GetCustomAttribute<AssemblyDescriptionAttribute>())
                .Bind(attr => attr.Description.EmptyAsNull())
                .Bind(desc => new ProgramDescriptionSection { Description = desc });
        }
    }
}
