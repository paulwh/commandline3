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

        public bool AutomaticHelpOutput { get; private set; }

        internal StringComparison StringComparison {
            get {
                return this.CaseSensitive ?
                    StringComparison.Ordinal :
                    StringComparison.OrdinalIgnoreCase;
            }
        }

        internal StringComparer StringComparer {
            get {
                return this.CaseSensitive ?
                    StringComparer.Ordinal :
                    StringComparer.OrdinalIgnoreCase;
            }
        }

        public ParserSettings(
            string longOptionPrefix = "--",
            char? shortOptionPrefix = '-',
            bool ignoreUnknownArguments = false,
            bool caseSensitive = true,
            bool allowPrefixMatch = false,
            bool automaticHelpOutput = true)
            : this(
                Console.Error,
                CultureInfo.InvariantCulture,
                longOptionPrefix,
                shortOptionPrefix,
                ignoreUnknownArguments,
                caseSensitive,
                allowPrefixMatch,
                automaticHelpOutput) {
        }

        public ParserSettings(
            TextWriter helpWriter,
            string longOptionPrefix = "--",
            char? shortOptionPrefix = '-',
            bool ignoreUnknownArguments = false,
            bool caseSensitive = true,
            bool allowPrefixMatch = false,
            bool automaticHelpOutput = true)
            : this(
                helpWriter,
                CultureInfo.InvariantCulture,
                longOptionPrefix,
                shortOptionPrefix,
                ignoreUnknownArguments,
                caseSensitive,
                allowPrefixMatch,
                automaticHelpOutput) {
        }
        public ParserSettings(
            CultureInfo parsingCulture,
            string longOptionPrefix = "--",
            char? shortOptionPrefix = '-',
            bool ignoreUnknownArguments = false,
            bool caseSensitive = true,
            bool allowPrefixMatch = false,
            bool automaticHelpOutput = true)
            : this(
                Console.Error,
                parsingCulture,
                longOptionPrefix,
                shortOptionPrefix,
                ignoreUnknownArguments,
                caseSensitive,
                allowPrefixMatch,
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
                        allowPrefixMatch: false
                    );
                case ArgumentStyle.Dos:
                    return new ParserSettings(
                        helpWriter,
                        parsingCulture,
                        longOptionPrefix: "/",
                        shortOptionPrefix: null,
                        ignoreUnknownArguments: true,
                        caseSensitive: false,
                        allowPrefixMatch: false
                    );
                case ArgumentStyle.Powershell:
                    return new ParserSettings(
                        helpWriter,
                        parsingCulture,
                        longOptionPrefix: "-",
                        shortOptionPrefix: null,
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
