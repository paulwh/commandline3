using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Core;
using CommandLine.Helpers;

namespace CommandLine.Text {
    public class OptionsSection : DefinitionListSection {
        public OptionsSection(IEnumerable<Definition> options)
            : base(options) {
        }

        private static Definition ToDefinition(ParserSettings settings,  OptionSpec option) {
            var termBuilder = new StringBuilder();
            var hasLongForm =
                !String.IsNullOrEmpty(settings.LongOptionPrefix) &&
                !String.IsNullOrEmpty(option.LongName);
            var hasShortForm =
                settings.ShortOptionPrefix.HasValue &&
                option.ShortName.HasValue;
            if (hasShortForm) {
                termBuilder.Append(settings.ShortOptionPrefix.Value);
                termBuilder.Append(option.ShortName.Value);
                if (hasLongForm) {
                    termBuilder.Append(", ");
                }
            }
            if (hasLongForm) {
                termBuilder.Append(settings.LongOptionPrefix);
                termBuilder.Append(option.LongName);
            }
            return new Definition(termBuilder.ToString(), option.HelpText);
        }

        internal static OptionsSection AutoBuild(ParserSettings settings, IEnumerable<OptionSpec> options) {
            if (!options.Any(IsHelpOption(settings))) {
                options = options.Append(
                    new OptionSpec("help", '?', settings.HelpTextResourceManager.GetString("HelpOptionDescription"))
                );
            }
            return new OptionsSection(options.Select(os => ToDefinition(settings, os))) {
                Indent = "    ",
                SubIndent = new String(' ', 36)
            };
        }

        private static Func<OptionSpec, bool> IsHelpOption(ParserSettings settings) {
            return os =>
                os.LongName.Equals("help", settings.StringComparison) ||
                os.ShortName.HasValue && (
                    os.ShortName == '?' ||
                    os.ShortName == 'h' ||
                    !settings.CaseSensitive && os.ShortName == 'H');
        }
    }
}
