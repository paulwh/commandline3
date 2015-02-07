using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine {
    public interface IOptionValueValidator {
        bool IsValidValue(OptionName option, object value, out string message);
    }
}
