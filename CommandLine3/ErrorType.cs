using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine {
    /// <summary>
    /// Discriminator enumeration of <see cref="CommandLine.Error"/> types.
    /// </summary>
    public enum ErrorType {
        /// <summary>
        /// Value of <see cref="CommandLine.MissingValueError"/> type.
        /// </summary>
        MissingValueError,
        /// <summary>
        /// Value of <see cref="CommandLine.UnknownOptionError"/> type.
        /// </summary>
        UnknownOptionError,
        /// <summary>
        /// Value of <see cref="CommandLine.UnexpectedValueError"/> type.
        /// </summary>
        UnexpectedValueError,
        /// <summary>
        /// Value of <see cref="CommandLine.MissingRequiredOptionError"/> type.
        /// </summary>
        MissingRequiredOptionError,
        /// <summary>
        /// Value of <see cref="CommandLine.DuplicateOptionError"/>
        /// </summary>
        DuplicateOptionError,
        /// <summary>
        /// Value of <see cref="CommandLine.MutuallyExclusiveSetError"/> type.
        /// </summary>
        MutuallyExclusiveSetError,
        /// <summary>
        /// Value of <see cref="CommandLine.BadValueFormatError"/> type.
        /// </summary>
        BadValueFormatError,
        /// <summary>
        /// Value of <see cref="CommandLine.InvalidValueError"/> type.
        /// </summary>
        InvalidValueError,
        /// <summary>
        /// Value of <see cref="CommandLine.NoVerbSelectedError"/> type.
        /// </summary>
        NoVerbSelectedError,
        /// <summary>
        /// Value of <see cref="CommandLine.BadVerbSelectedError"/> type.
        /// </summary>
        BadVerbSelectedError,
        /// <summary>
        /// Value of <see cref="CommandLine.HelpRequestedError"/> type.
        /// </summary>
        HelpRequestedError,
        /// <summary>
        /// Value of <see cref="CommandLine.HelpVerbRequestedError"/> type.
        /// </summary>
        HelpVerbRequestedError
    }
}
