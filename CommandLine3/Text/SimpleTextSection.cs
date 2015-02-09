using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Text {
    public class SimpleTextSection : HelpTextSection {
        public static readonly HelpTextSection Blank =
            new SimpleTextSection(String.Empty);

        public List<string> Lines { get; private set; }

        public SimpleTextSection(params string[] lines) {
            this.Lines = new List<string>(lines);
        }

        protected override IEnumerable<string> RenderLines() {
            return this.Lines;
        }
    }
}
