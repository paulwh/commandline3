using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Helpers {
    public static class EnumerableExtensions {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T newValue) {
            foreach (var value in enumerable) {
                yield return value;
            }
            yield return newValue;
        }
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> enumerable, T newValue) {
            yield return newValue;
            foreach (var value in enumerable) {
                yield return value;
            }
        }
    }
}
