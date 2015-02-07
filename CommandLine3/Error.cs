using System;
using System.Linq;

namespace CommandLine {
    public abstract class Error {
        public ErrorType Type { get; private set; }

        internal Error(ErrorType type) {
            this.Type = type;
        }
    }

    public abstract class OptionError : Error {
        public OptionName Option { get; private set; }

        internal OptionError(ErrorType type, OptionName option)
            : base(type) {

            if (option == null) {
                throw new ArgumentNullException("option");
            }

            this.Option = option;
        }
    }

    public abstract class ValueError : OptionError {
        public string Value { get; private set; }

        internal ValueError(ErrorType type, OptionName option, string value)
            : base(type, option) {

            if (value == null) {
                throw new ArgumentNullException("value");
            }

            this.Value = value;
        }
    }

    public class MissingValueError : OptionError {
        public MissingValueError(OptionName option)
            : base(ErrorType.MissingValueError, option) {
        }
    }

    public class UnknownOptionError : OptionError {
        public UnknownOptionError(OptionName option)
            : base(ErrorType.UnknownOptionError, option) {
        }
    }

    public class MissingRequiredOptionError : OptionError {
        public MissingRequiredOptionError(OptionName option)
            : base(ErrorType.MissingRequiredOptionError, option) {
        }
    }

    public class MutuallyExclusiveSetError : Error {
        public MutuallyExclusiveSetError(IGrouping<string, OptionName> confictingOptions)
            : base(ErrorType.MutuallyExclusiveSetError) {
        }
    }

    public class BadValueFormatError : ValueError {
        public string Message { get; private set; }
        public BadValueFormatError(OptionName option, string value, string message)
            : base(ErrorType.BadValueFormatError, option, value) {

            if (message == null) {
                throw new ArgumentNullException("message");
            }

            this.Message = message;
        }
    }

    public class InvalidValueError : ValueError {
        public object ParsedValue { get; private set; }
        public string Message { get; private set; }

        public InvalidValueError(OptionName option, string value, object parsedValue, string message)
            : base(ErrorType.InvalidValueError, option, value) {

            if (message == null) {
                throw new ArgumentNullException("message");
            }

            this.Message = message;
            this.ParsedValue = parsedValue;
        }
    }

    public class NoVerbSelectedError : Error {
        public static readonly NoVerbSelectedError Instance = new NoVerbSelectedError();

        private NoVerbSelectedError()
            : base(ErrorType.NoVerbSelectedError) {
        }
    }

    public class BadVerbSelectedError : Error {
        public static readonly BadVerbSelectedError Instance = new BadVerbSelectedError();

        private BadVerbSelectedError()
            : base(ErrorType.BadVerbSelectedError) {
        }
    }

    public class HelpRequestedError : Error {
        public static readonly HelpRequestedError Instance = new HelpRequestedError();

        private HelpRequestedError()
            : base(ErrorType.HelpRequestedError) {
        }
    }

    public class HelpVerbRequestedError : Error {
        public static readonly HelpVerbRequestedError Instance = new HelpVerbRequestedError();

        private HelpVerbRequestedError()
            : base(ErrorType.HelpVerbRequestedError) {
        }
    }
}
