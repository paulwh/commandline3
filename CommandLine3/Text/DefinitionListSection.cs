using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLine.Text {
    public class Definition {
        public string Term { get; private set; }
        public string Description { get; private set; }

        public Definition(string term, string definition) {
            if (term == null) {
                throw new ArgumentNullException("term");
            }

            this.Term = term;
            this.Description = definition ?? String.Empty;
        }
    }

    public class DefinitionListSection : HelpTextSection {
        public List<Definition> Definitions { get; private set; }
        public int TermMinimumWidth { get; private set; }

        public DefinitionListSection(params Definition[] definitions)
            : this((IEnumerable<Definition>)definitions) {
        }

        public DefinitionListSection(IEnumerable<Definition> definitions) {
            this.Definitions = new List<Definition>(definitions);
            this.TermMinimumWidth = 32;
            // 12345678901234567890123456789012    Description text...
        }

        protected override IEnumerable<string> RenderLines() {
            var sb = new StringBuilder();
            foreach (var definition in this.Definitions) {
                var term = definition.Term;
                var termLength = term.Length;
                sb.Append(term);
                if (termLength < this.TermMinimumWidth - 1) {
                    // add padding up to min width
                    sb.Append(new String(' ', this.TermMinimumWidth - termLength));
                } else {
                    // allowing for at least 2 spaces, and aligning to columns
                    // at intervales of 4 spaces, add the minimal necessary
                    // padding.
                    var padding = 5 - ((termLength - (this.TermMinimumWidth - 1)) % 4);
                    sb.Append(new String(' ', padding));
                }
                sb.Append(definition.Description);
                yield return sb.ToString();
                sb.Clear();
            }
        }
    }
}
