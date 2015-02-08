using System;
using System.Runtime.Serialization;

namespace CommandLine {
    [Serializable]
    public class DeserializationException : Exception {
        public string Value { get; private set; }

        public DeserializationException(string value, string message)
            : base(message) {

            this.Value = value;
        }

        public DeserializationException(string value, string message, Exception innerException)
            : base(message, innerException) {

            this.Value = value;
        }

        protected DeserializationException(SerializationInfo info, StreamingContext context)
            : base(info, context) {

            this.Value = info.GetString("value");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("value", this.Value);
        }
    }
}
