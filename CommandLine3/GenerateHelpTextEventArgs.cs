using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Text;

namespace CommandLine {
    public class GenerateHelpTextEventArgs : EventArgs {
        public HelpText HelpText { get; set; }
    }
}
