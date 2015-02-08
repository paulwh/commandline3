using System;

namespace CommandLine.Core {
    internal class VerbSpec {
        public string VerbName { get; private set; }
        public string HelpText { get; private set; }
        public Type VerbType { get; private set; }
        public Func<object> CreateInstance { get; private set; }

        public VerbSpec(string verbName, string helpText, Type verbType, Func<object> createInstance) {
            this.VerbType = verbType;
            this.CreateInstance = createInstance;
        }
    }
}
