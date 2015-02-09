using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandLine.Text {
    public class HelpText : IEnumerable<HelpTextSection> {
        public ProgramNameSection ProgramName { get; set; }
        public ProgramUsageSection ProgramUsage { get; set; }
        public ProgramDescriptionSection ProgramDescription { get; set; }
        public HelpTextSection LongDescription { get; set; }
        public SelectedCommandSection SelectedCommand { get; set; }
        public ParseErrorsSection Errors { get; set; }
        public HelpTextSection PreOptionsSection { get; set; }
        public OptionsSection Options { get; set; }
        public VerbsSection Verbs { get; set; }
        public HelpTextSection PostOptionsSection { get; set; }
        public ProgramCopyrightSection Copyright { get; set; }

        private IEnumerable<HelpTextSection> AsEnumerable() {
            yield return this.ProgramName;
            yield return this.ProgramUsage;
            if (this.ProgramName != null || this.ProgramUsage != null) {
                yield return SimpleTextSection.Blank;
            }
            if (this.ProgramDescription != null) {
                yield return this.ProgramDescription;
                yield return SimpleTextSection.Blank;
            }
            if (this.LongDescription != null) {
                yield return this.LongDescription;
                yield return SimpleTextSection.Blank;
            }
            if (this.SelectedCommand != null) {
                yield return this.SelectedCommand;
                yield return SimpleTextSection.Blank;
            }
            if (this.Errors != null) {
                yield return this.Errors;
                yield return SimpleTextSection.Blank;
            }
            yield return this.PreOptionsSection;
            yield return this.Options;
            yield return this.Verbs;
            if ((this.Options != null || this.Verbs != null) &&
                (this.PostOptionsSection != null | this.Copyright != null)) {
                yield return SimpleTextSection.Blank;
            }
            yield return this.PostOptionsSection;
            yield return this.Copyright;
        }

        public IEnumerator<HelpTextSection> GetEnumerator() {
            return this.AsEnumerable().Where(s => s != null).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void Render(TextWriter writer) {
            foreach (var section in this) {
                section.Render(writer);
            }
        }

        public static HelpText AutoBuild<T>(ParserSettings settings, ParserResult<T> result) {
            var displayVerbHelp = result.IsVerbErrorResult();
            var helpRequested = result.Errors.Any(e => e.Type == ErrorType.HelpRequestedError);
            return new HelpText {
                ProgramName = ProgramNameSection.AutoBuild(settings),
                ProgramUsage = displayVerbHelp ? ProgramUsageSection.AutoBuild(settings, result.VerbTypes) : ProgramUsageSection.AutoBuild(settings, result.Options),
                ProgramDescription = ProgramDescriptionSection.AutoBuild(),
                SelectedCommand = !displayVerbHelp ? SelectedCommandSection.AutoBuild(settings, result) : null,
                Errors = !helpRequested ? ParseErrorsSection.AutoBuild(settings, result.Errors) : null,
                Options = !displayVerbHelp ? OptionsSection.AutoBuild(settings, result.Options) : null,
                Verbs = displayVerbHelp ? VerbsSection.AutoBuild(settings, result.VerbTypes) : null,
                Copyright = ProgramCopyrightSection.AutoBuild()
            };
        }
    }
}
