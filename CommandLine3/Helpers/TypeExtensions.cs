using System;
using System.Linq;

namespace CommandLine.Helpers {
    public static class TypeExtensions {
        public static TAttribute GetCustomAttribute<TAttribute>(this Type type, bool inherit)
            where TAttribute : Attribute {

            return
                type.GetCustomAttributes(typeof(TAttribute), inherit)
                    .Cast<TAttribute>()
                    .FirstOrDefault();
        }

        /// <summary>
        /// Checks if a type implements an interface type, or implements a
        /// concrete form of a generic interface definition.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method differes from IsAssignableFrom, in addition to
        /// inverting the order of the types, in that while
        /// typeof(ICollection&lt;>).IsAssignableFrom(typeof(List&lt;String>))
        /// would return false, this method applied to those types in reverse
        /// would return true.
        /// </para>
        /// <para>
        /// Additional the <paramref name="interfaceType"/> parameter must be
        /// an interface type.
        /// </para>
        /// </remarks>
        public static bool Implements(this Type type, Type interfaceType) {
            if (interfaceType == null) {
                throw new ArgumentNullException("interfaceType");
            } else if (!interfaceType.IsInterface) {
                throw new ArgumentException(
                    "The interfaceType parameter must be an interface.",
                    "interfaceType"
                );
            }
            var isGeneric = interfaceType.IsGenericTypeDefinition;
            foreach (var implemented in type.GetInterfaces()) {
                if (isGeneric) {
                    if (implemented.IsGenericType &&
                        implemented.GetGenericTypeDefinition() == interfaceType) {
                        return true;
                    }
                } else if (implemented == interfaceType) {
                    return true;
                }
            }

            return false;
        }

        public static Type GetInterface(this Type type, Type interfaceType) {
            if (interfaceType == null) {
                throw new ArgumentNullException("interfaceType");
            } else if (!interfaceType.IsInterface) {
                throw new ArgumentException(
                    "The interfaceType parameter must be an interface.",
                    "interfaceType"
                );
            }
            var isGeneric = interfaceType.IsGenericTypeDefinition;
            foreach (var implemented in type.GetInterfaces()) {
                if (isGeneric) {
                    if (implemented.IsGenericType &&
                        implemented.GetGenericTypeDefinition() == interfaceType) {
                        return implemented;
                    }
                } else if (implemented == interfaceType) {
                    return implemented;
                }
            }

            return null;
        }

        public static bool IsOfType(this Type type, Type baseType) {
            if (baseType == null) {
                throw new ArgumentNullException("isOfType");
            } else if (baseType.IsInterface) {
                throw new ArgumentException(
                    "The baseType parameter must bot be an interface.",
                    "baseType"
                );
            }
            var isGeneric = baseType.IsGenericTypeDefinition;
            var current = type;
            while (current != null) {
                if (isGeneric) {
                    if (current.IsGenericType && current.GetGenericTypeDefinition() == baseType) {
                        return true;
                    }
                } else {
                    if (current == baseType) {
                        return true;
                    }
                }
                current = current.BaseType;
            }
            return false;
        }
    }
}
