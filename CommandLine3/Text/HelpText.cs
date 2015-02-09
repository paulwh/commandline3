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
            yield return this.SelectedCommand;
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

        private static readonly ErrorType[] VerbHelpErrorTypes =
            new [] { ErrorType.HelpVerbRequestedError, ErrorType.BadVerbSelectedError, ErrorType.NoVerbSelectedError };

        public static HelpText AutoBuild<T>(ParserSettings settings, ParserResult<T> result) {
            var displayVerbHelp = result.Errors.Any(e => VerbHelpErrorTypes.Contains(e.Type));
            return new HelpText {
                ProgramName = ProgramNameSection.AutoBuild(settings),
                ProgramUsage = ProgramUsageSection.AutoBuild(settings, result.Options),
                ProgramDescription = ProgramDescriptionSection.AutoBuild(),
                SelectedCommand = result.Verb != null && !displayVerbHelp ? new SelectedCommandSection { Verb = result.Verb } : null,
                Errors = ParseErrorsSection.AutoBuild(settings, result.Errors),
                Options = !displayVerbHelp ? OptionsSection.AutoBuild(settings, result.Options) : null,
                Verbs = displayVerbHelp ? VerbsSection.AutoBuild(settings, result.VerbTypes) : null,
                Copyright = ProgramCopyrightSection.AutoBuild()
            };
        }
    }
}
