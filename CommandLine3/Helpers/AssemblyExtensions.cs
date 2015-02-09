using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine.Helpers {
    public static class AssemblyExtensions {
        public static TAttribute GetCustomAttribute<TAttribute>(this Assembly assembly, bool inherit = false)
            where TAttribute : Attribute {

            return
                assembly.GetCustomAttributes(typeof(TAttribute), inherit)
                    .Cast<TAttribute>()
                    .FirstOrDefault();
        }
    }
}
