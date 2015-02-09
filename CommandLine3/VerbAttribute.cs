using System;

namespace CommandLine {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class VerbAttribute : Attribute {
        public string Name { get; private set; }
        public string HelpText { get; set; }

        public VerbAttribute(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            } else if (String.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("Name cannot be blank.", "name");
            }

            this.Name = name.Trim();
        }
    }
}
