using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Core;
using CommandLine.Helpers;

namespace CommandLine.Text {
    public class VerbsSection : DefinitionListSection {
        public VerbsSection(IEnumerable<Definition> verbs)
            : base(verbs) {
        }

        private static Definition ToDefinition(VerbSpec verb) {
            return new Definition(verb.VerbName, verb.HelpText);
        }

        internal static VerbsSection AutoBuild(ParserSettings settings, IEnumerable<VerbSpec> verbs) {
            if (!verbs.Any(v => v.VerbName.Equals("help", settings.StringComparison))) {
                verbs = verbs.Append(
                    new VerbSpec("help", settings.HelpTextResourceManager.GetString("HelpVerbDescription"))
                );
            }
            return new VerbsSection(verbs.Select(vs => ToDefinition(vs))) {
                Indent = "    ",
                SubIndent = new String(' ', 36)
            };
        }
    }
}
