using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Text {
    public class ParseErrorsSection : HelpTextSection {
        public List<Error> Errors { get; private set; }
        public ParserSettings Settings { get; private set; }

        public ParseErrorsSection(ParserSettings settings, params Error[] errors)
            : this(settings, (IEnumerable<Error>)errors) {
        }

        public ParseErrorsSection(ParserSettings settings, IEnumerable<Error> errors) {
            if (settings == null) {
                throw new ArgumentNullException("settings");
            } else if (errors == null) {
                throw new ArgumentNullException("errors");
            }
            this.Errors = new List<Error>(errors);
        }

        protected override IEnumerable<string> RenderLines() {
            foreach (var error in this.Errors) {
                yield return error.ToString(this.Settings);
            }
        }

        private static readonly ErrorType[] HelpRequestErrorTypes =
            new[] { ErrorType.HelpRequestedError, ErrorType.HelpVerbRequestedError };

        internal static ParseErrorsSection AutoBuild(ParserSettings settings, IEnumerable<Error> errors) {
            var actualErrors = errors.Where(e => !HelpRequestErrorTypes.Contains(e.Type));
            if (actualErrors.Any()) {
                return new ParseErrorsSection(settings, actualErrors);
            } else {
                return null;
            }
        }
    }
}
