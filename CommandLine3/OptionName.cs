using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine {
    public class OptionName {
        public string LongName { get; private set; }

        public char? ShortName { get; private set; }

        public OptionName(string longName, char? shortName) {
            this.LongName = longName;
            this.ShortName = shortName;
        }
    }
}
