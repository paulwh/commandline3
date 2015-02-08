using System;
using System.Linq;
using System.Reflection;

namespace CommandLine.Helpers {
    public static class PropertyInfoExtensions {
        public static TAttribute GetCustomAttribute<TAttribute>(this PropertyInfo property, bool inherit = false)
            where TAttribute : Attribute {

            return property
                .GetCustomAttributes(typeof(TAttribute), inherit)
                .Cast<TAttribute>()
                .FirstOrDefault();
        }

        public static object GetValue(this PropertyInfo property, object instance) {
            return property.GetValue(instance, new object[0]);
        }

        public static void SetValue(this PropertyInfo property, object instance, object value) {
            property.SetValue(instance, value, new object[0]);
        }
    }
}
