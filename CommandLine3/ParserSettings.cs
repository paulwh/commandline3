using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using CommandLine.Helpers;

namespace CommandLine {
    public enum ArgumentStyle {
        Unix,
        Dos,
        Powershell
    }

    public class ParserSettings {
        internal static readonly ResourceManager DefaultResourceManager =
            new ResourceManager("CommandLine.Text.HelpTextStrings", typeof(ParserSettings).Assembly);

        public TextWriter HelpWriter { get; private set; }

        public CultureInfo ParsingCulture { get; private set; }

        public ResourceManager HelpTextResourceManager { get; private set; }

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

        internal IEqualityComparer<char> CharacterComparer {
            get {
                return this.CaseSensitive ?
                    CharComparer.Ordinal :
                    CharComparer.OrdinalIgnoreCase;
            }
        }

        private class CharComparer : EqualityComparer<char>, IComparer<char> {
            public static CharComparer Ordinal = new CharComparer();
            public static readonly CharComparer OrdinalIgnoreCase =
                new CharComparer { ignoreCase = true };

            private bool ignoreCase;

            private CharComparer() {
            }

            public int Compare(char x, char y) {
                return ignoreCase ?
                    Char.ToLowerInvariant(x).CompareTo(Char.ToLowerInvariant(y)) :
                    x.CompareTo(y);
            }

            public override bool Equals(char x, char y) {
                return ignoreCase ?
                    Char.ToLowerInvariant(x).Equals(Char.ToLowerInvariant(y)) :
                    x.Equals(y);
            }

            public override int GetHashCode(char obj) {
                return ignoreCase ?
                    Char.ToLowerInvariant(obj).GetHashCode() :
                    obj.GetHashCode();
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
                DefaultResourceManager,
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
                DefaultResourceManager,
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
                DefaultResourceManager,
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
            bool automaticHelpOutput = true)
            : this(
                helpWriter,
                parsingCulture,
                DefaultResourceManager,
                longOptionPrefix,
                shortOptionPrefix,
                ignoreUnknownArguments,
                caseSensitive,
                allowPrefixMatch,
                automaticHelpOutput) {
        }

        public ParserSettings(
            ResourceManager helpTextResourceManager,
            string longOptionPrefix = "--",
            char? shortOptionPrefix = '-',
            bool ignoreUnknownArguments = false,
            bool caseSensitive = true,
            bool allowPrefixMatch = false,
            bool automaticHelpOutput = true)
            : this(
                Console.Error,
                CultureInfo.InvariantCulture,
                helpTextResourceManager,
                longOptionPrefix,
                shortOptionPrefix,
                ignoreUnknownArguments,
                caseSensitive,
                allowPrefixMatch,
                automaticHelpOutput) {
        }

        public ParserSettings(
            TextWriter helpWriter,
            ResourceManager helpTextResourceManager,
            string longOptionPrefix = "--",
            char? shortOptionPrefix = '-',
            bool ignoreUnknownArguments = false,
            bool caseSensitive = true,
            bool allowPrefixMatch = false,
            bool automaticHelpOutput = true)
            : this(
                helpWriter,
                CultureInfo.InvariantCulture,
                helpTextResourceManager,
                longOptionPrefix,
                shortOptionPrefix,
                ignoreUnknownArguments,
                caseSensitive,
                allowPrefixMatch,
                automaticHelpOutput) {
        }

        public ParserSettings(
            CultureInfo parsingCulture,
            ResourceManager helpTextResourceManager,
            string longOptionPrefix = "--",
            char? shortOptionPrefix = '-',
            bool ignoreUnknownArguments = false,
            bool caseSensitive = true,
            bool allowPrefixMatch = false,
            bool automaticHelpOutput = true)
            : this(
                Console.Error,
                parsingCulture,
                helpTextResourceManager,
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
            ResourceManager helpTextResourceManager,
            string longOptionPrefix = "--",
            char? shortOptionPrefix = '-',
            bool ignoreUnknownArguments = false,
            bool caseSensitive = true,
            bool allowPrefixMatch = false,
            bool automaticHelpOutput = true) {

            this.HelpWriter = helpWriter;
            this.ParsingCulture = parsingCulture;
            this.HelpTextResourceManager = helpTextResourceManager;
            this.LongOptionPrefix = longOptionPrefix;
            this.ShortOptionPrefix = shortOptionPrefix;
            this.IgnoreUnknownArguments = ignoreUnknownArguments;
            this.CaseSensitive = caseSensitive;
            this.AllowPrefixMatch = allowPrefixMatch;
            this.AutomaticHelpOutput = automaticHelpOutput;
        }

        public ParserSettings With(
            TextWriter helpWriter = null,
            CultureInfo parsingCulture = null,
            ResourceManager helpTextResourceManager = null,
            Maybe<string> longOptionPrefix = default(Maybe<string>),
            Maybe<char?> shortOptionPrefix = default(Maybe<char?>),
            Maybe<bool> ignoreUnknownArguments = default(Maybe<bool>),
            Maybe<bool> caseSensitive = default(Maybe<bool>),
            Maybe<bool> allowPrefixMatch = default(Maybe<bool>),
            Maybe<bool> automaticHelpOutput = default(Maybe<bool>)) {

            return new ParserSettings(
                helpWriter ?? this.HelpWriter,
                parsingCulture ?? this.ParsingCulture,
                helpTextResourceManager ?? this.HelpTextResourceManager,
                longOptionPrefix.GetValueOrDefault(this.LongOptionPrefix),
                shortOptionPrefix.GetValueOrDefault(this.ShortOptionPrefix),
                ignoreUnknownArguments.GetValueOrDefault(this.IgnoreUnknownArguments),
                caseSensitive.GetValueOrDefault(this.CaseSensitive),
                automaticHelpOutput.GetValueOrDefault(this.AutomaticHelpOutput)
            );
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
