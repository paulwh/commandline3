using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Helpers;

namespace CommandLine.Core {
    internal class OptionLookup {
        public IList<OptionSpec> All { get; private set; }
        public IDictionary<string, OptionSpec> ByLongName { get; private set; }
        public IDictionary<char, OptionSpec> ByShortName { get; private set; }
        public IList<OptionSpec> ByPosition { get; private set; }

        public OptionLookup(
            IList<OptionSpec> all,
            IDictionary<string, OptionSpec> byLongName,
            IDictionary<char, OptionSpec> byShortName,
            IList<OptionSpec> byPosition) {

            this.All = all;
            this.ByLongName = byLongName;
            this.ByShortName = byShortName;
            this.ByPosition = byPosition;
        }

        public OptionSpec ForToken(Token token) {
            switch (token.Type) {
                case TokenType.Option:
                case TokenType.OptionExpectingValue:
                    OptionSpec spec;
                    return this.ByLongName.TryGetValue(token.Value, out spec) ?
                        spec :
                        null;
                case TokenType.ShortOption:
                    return this.ByShortName.TryGetValue(token.Value[0], out spec) ?
                        spec :
                        null;
                default:
                    throw new NotSupportedException("TokenType " + token.Type + " not supported.");
            }
        }

        public static OptionLookup ForType<T>() {
            var optionSpecs =
                typeof(T)
                    .GetProperties()
                    .Select(pi => new OptionSpec(pi, pi.GetCustomAttribute<OptionAttribute>(inherit: false)))
                    .ToList();

            var longNameIndex =
                optionSpecs.ToDictionary(os => os.LongName);

            var shortNameIndex =
                optionSpecs
                    .Where(os => os.ShortName.HasValue)
                    .ToDictionary(os => os.ShortName.Value);

            var byPosition =
                optionSpecs
                    .Where(os => os.Position.HasValue)
                    .GroupBy(os => os.Position.Value)
                    .Select(
                        grp => {
                            try {
                                return grp.Single();
                            } catch {
                                throw new InvalidOperationException(
                                    "Multiple options have the same position: " + String.Join(",", grp.Select(os => os.LongName))
                                );
                            }
                        })
                    .OrderBy(os => os.Position.Value)
                    .ToList();

            var optionLookup =
                new OptionLookup(
                    optionSpecs,
                    longNameIndex,
                    shortNameIndex,
                    byPosition
                );
            return optionLookup;
        }
    }
}
