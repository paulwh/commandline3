using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Core {
    internal enum TokenType {
        /// <summary>
        /// A standard long form option.
        /// </summary>
        Option,
        /// <summary>
        /// A long form option followed by an equal sign (i.e. --foo=bar)
        /// </summary>
        OptionExpectingValue,
        /// <summary>
        /// A short for option.
        /// </summary>
        ShortOption,
        /// <summary>
        /// A value token
        /// </summary>
        Value
    }

    internal class Token {
        public TokenType Type { get; private set; }
        public string RawValue { get; private set; }
        public string Value { get; private set; }

        public Token(TokenType type, string rawValue, string value) {
            this.Type = type;
            this.RawValue = rawValue;
            this.Value = value;
        }
    }
}
