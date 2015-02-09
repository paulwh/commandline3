using System;

namespace CommandLine.Helpers {
    public static class StringExtensions {
        public static string EmptyAsNull(this string value) {
            return String.IsNullOrEmpty(value) ? null : value;
        }
    }
}
