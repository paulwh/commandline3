using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public abstract class ValueError : Error {
        public string Value { get; private set; }

        internal ValueError(ErrorType type, string value)
            : base(type) {

            this.Value = value;
        }
    }

    public abstract class OptionValueError : OptionError {
        public string Value { get; private set; }

        internal OptionValueError(ErrorType type, OptionName option, string value)
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

    public class UnexpectedValueError : ValueError {
        public UnexpectedValueError(string value)
            : base(ErrorType.UnexpectedValueError, value) {
        }
    }

    public class MissingRequiredOptionError : OptionError {
        public MissingRequiredOptionError(OptionName option)
            : base(ErrorType.MissingRequiredOptionError, option) {
        }
    }

    public class DuplicateOptionError : OptionValueError {
        public DuplicateOptionError(OptionName option, string value)
            : base(ErrorType.DuplicateOptionError, option, value) {
        }
    }

    public class MutuallyExclusiveSetError : Error {
        public IList<IGrouping<string, OptionName>> ConflictingOptions { get; private set; }

        public MutuallyExclusiveSetError(IEnumerable<IGrouping<string, OptionName>> confictingOptions)
            : base(ErrorType.MutuallyExclusiveSetError) {

            this.ConflictingOptions =
                new ReadOnlyCollection<IGrouping<string, OptionName>>(confictingOptions.ToList());
        }
    }

    public class BadValueFormatError : OptionValueError {
        public string Message { get; private set; }
        public BadValueFormatError(OptionName option, string value, string message)
            : base(ErrorType.BadValueFormatError, option, value) {

            if (message == null) {
                throw new ArgumentNullException("message");
            }

            this.Message = message;
        }
    }

    public class InvalidValueError : OptionValueError {
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
        public string Verb { get; private set; }

        public BadVerbSelectedError(string verb)
            : base(ErrorType.BadVerbSelectedError) {

            this.Verb = verb;
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
