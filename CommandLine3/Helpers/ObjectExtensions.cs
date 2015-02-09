using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Helpers {
    public static class ObjectExtensions {
        public static TOut Bind<TIn, TOut>(this TIn value, Func<TIn, TOut> func)
            where TIn : class
            where TOut : class {

            return value != null ? func(value) : null;
        }
    }
}
