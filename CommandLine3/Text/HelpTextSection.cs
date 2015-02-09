using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CommandLine.Text {
    public abstract class HelpTextSection {
        public static int DefaultAutoWrap { get; set; }

        public string Indent { get; set; }

        public int? autoWrap;
        public int AutoWrap {
            get { return this.autoWrap.GetValueOrDefault(DefaultAutoWrap); }
            set { this.autoWrap = value; }
        }

        public string SubIndent { get; set; }

        static HelpTextSection() {
            DefaultAutoWrap = 80;
        }

        public void Render(TextWriter writer) {
            foreach (var line in this.RenderLines().SelectMany(line => line.Split('\n'))) {
                line.Replace("\t", "    ");
                if (!String.IsNullOrEmpty(this.Indent)) {
                    writer.Write(this.Indent);
                }
                if (this.AutoWrap > 0 &&
                    line.Length + (this.Indent ?? String.Empty).Length > AutoWrap) {

                    var first = true;
                    var pos = 0;
                    do {
                        int indentWidth;
                        if (first) {
                            indentWidth = (this.Indent ?? String.Empty).Length;
                            first = false;
                        } else {
                            indentWidth = (this.SubIndent ?? String.Empty).Length;
                            if (indentWidth > 0) {
                                writer.Write(indentWidth);
                            }
                        }
                        var nextPos = pos + AutoWrap - indentWidth;
                        if (nextPos > line.Length) {
                            // we're done, we've got enough space for the rest of the line
                            nextPos = line.Length;
                        } else {
                            while (nextPos > pos && Char.IsLetterOrDigit(line[nextPos - 1])) {
                                nextPos--;
                            }
                            if (nextPos == pos) {
                                // there's no good places to break the line, so just break the word in half.
                                nextPos = pos + AutoWrap - indentWidth;
                            }
                        }
                        writer.WriteLine(line.Substring(pos, nextPos - 1));
                        pos = nextPos;
                        // skip over any whitespace.
                        while (pos < line.Length && Char.IsWhiteSpace(line[pos])) {
                            pos++;
                        }
                    } while (pos < line.Length);
                } else {
                    writer.WriteLine(line);
                }
            }
        }

        protected abstract IEnumerable<string> RenderLines();
    }
}
