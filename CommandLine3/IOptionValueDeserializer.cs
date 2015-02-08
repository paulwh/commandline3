using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CommandLine {
    public interface IOptionValueDeserializer {
        bool AcceptsMultipleValues { get; }
        object CreateInstance();
        object Deserialize(CultureInfo parsingCulture, object instance, string value);
        object Deserialize(CultureInfo parsingCulture, object instance, IEnumerable<string> values);
    }
}
