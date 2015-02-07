using System;
using System.Globalization;
using System.IO;

namespace CommandLine {
    public enum ArgumentStyle {
        Unix,
        Dos,
        Powershell
    }

    public class ParserSettings {
        public TextWriter HelpWriter { get; private set; }

        public CultureInfo ParsingCulture { get; private set; }

        public string LongOptionPrefix { get; private set; }

        public char? ShortOptionPrefix { get; private set; }

        public bool IgnoreUnknownArguments { get; private set; }

        public bool CaseSensitive { get; private set; }

        public bool AllowPrefixMatch { get; private set; }

        public bool SupportFlags { get; private set; }

        public bool AutomaticHelpOutput { get; private set; }

        public ParserSettings(
            string longOptionPrefix = "--",
            char? shortOptionPrefix = '-',
            bool ignoreUnknownArguments = false,
            bool caseSensitive = true,
            bool allowPrefixMatch = false,
            bool supportFlags = true,
            bool automaticHelpOutput = true)
            : this(
                Console.Error,
                CultureInfo.InvariantCulture,
                longOptionPrefix,
                shortOptionPrefix,
                ignoreUnknownArguments,
                caseSensitive,
                allowPrefixMatch,
                supportFlags,
                automaticHelpOutput) {
        }

        public ParserSettings(
            TextWriter helpWriter,
            string longOptionPrefix = "--",
            char? shortOptionPrefix = '-',
            bool ignoreUnknownArguments = false,
            bool caseSensitive = true,
            bool allowPrefixMatch = false,
            bool supportFlags = true,
            bool automaticHelpOutput = true)
            : this(
                helpWriter,
                CultureInfo.InvariantCulture,
                longOptionPrefix,
                shortOptionPrefix,
                ignoreUnknownArguments,
                caseSensitive,
                allowPrefixMatch,
                supportFlags,
                automaticHelpOutput) {
        }
        public ParserSettings(
            CultureInfo parsingCulture,
            string longOptionPrefix = "--",
            char? shortOptionPrefix = '-',
            bool ignoreUnknownArguments = false,
            bool caseSensitive = true,
            bool allowPrefixMatch = false,
            bool supportFlags = true,
            bool automaticHelpOutput = true)
            : this(
                Console.Error,
                parsingCulture,
                longOptionPrefix,
                shortOptionPrefix,
                ignoreUnknownArguments,
                caseSensitive,
                allowPrefixMatch,
                supportFlags,
                automaticHelpOutput) {
        }

        public ParserSettings(
            TextWriter helpWriter,
            CultureInfo parsingCulture,
            string longOptionPrefix = "--",
            char? shortOptionPrefix = '-',
            bool ignoreUnknownArguments = false,
            bool caseSensitive = true,
            bool allowPrefixMatch = false,
            bool supportFlags = true,
            bool automaticHelpOutput = true) {
        }
        
        public static ParserSettings ForArgumentStyle(ArgumentStyle style) {
            return ForArgumentStyle(Console.Error, CultureInfo.InvariantCulture, style);
        }

        public static ParserSettings ForArgumentStyle(TextWriter helpWriter, ArgumentStyle style) {
            return ForArgumentStyle(helpWriter, CultureInfo.InvariantCulture, style);
        }

        public static ParserSettings ForArgumentStyle(CultureInfo parsingCulture, ArgumentStyle style) {
            return ForArgumentStyle(Console.Error, parsingCulture, style);
        }

        public static ParserSettings ForArgumentStyle(
            TextWriter helpWriter,
            CultureInfo parsingCulture,
            ArgumentStyle style) {

            switch (style) {
                case ArgumentStyle.Unix:
                    return new ParserSettings(
                        helpWriter,
                        parsingCulture,
                        longOptionPrefix: "--",
                        shortOptionPrefix: '-',
                        ignoreUnknownArguments: false,
                        caseSensitive: true,
                        allowPrefixMatch: false,
                        supportFlags: true
                    );
                case ArgumentStyle.Dos:
                    return new ParserSettings(
                        helpWriter,
                        parsingCulture,
                        longOptionPrefix: "/",
                        shortOptionPrefix: '/',
                        ignoreUnknownArguments: true,
                        supportFlags: false,
                        caseSensitive: false,
                        allowPrefixMatch: false
                    );
                case ArgumentStyle.Powershell:
                    return new ParserSettings(
                        helpWriter,
                        parsingCulture,
                        longOptionPrefix: "-",
                        shortOptionPrefix: null,
                        supportFlags: false,
                        caseSensitive: false,
                        allowPrefixMatch: true
                    );
                default:
                    throw new ArgumentException(
                        "Unrecognized argument style: " + style.ToString(),
                        "style"
                    );
            }
        }
    }
}
