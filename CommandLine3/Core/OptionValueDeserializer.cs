using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using CommandLine.Helpers;

namespace CommandLine.Core {
    internal static class OptionValueDeserializer {
        public static IOptionValueDeserializer ForType(Type type) {
            return (IOptionValueDeserializer)
                ForTypeMethod.MakeGenericMethod(type).Invoke(null, new object[0]);
        }

        private static MethodInfo ForTypeMethod =
            ReflectionHelper.GetGenericMethodDefinition(() => ForType<object>());

        public static IOptionValueDeserializer ForType<T>() {
            return OptionValueDeserializer<T>.Default;
        }

        internal static readonly IDictionary<Type, IOptionValueDeserializer> PrimitiveDeserializers =
            new Dictionary<Type, IOptionValueDeserializer> {
                { typeof(object), StringDeserializer.Instance },
                { typeof(string), StringDeserializer.Instance },
                { typeof(byte), ConvertPrimitivesDeserializer<byte>.Instance },
                { typeof(sbyte), ConvertPrimitivesDeserializer<sbyte>.Instance },
                { typeof(uint), ConvertPrimitivesDeserializer<uint>.Instance },
                { typeof(int), ConvertPrimitivesDeserializer<int>.Instance },
                { typeof(ulong), ConvertPrimitivesDeserializer<ulong>.Instance },
                { typeof(long), ConvertPrimitivesDeserializer<long>.Instance },
                { typeof(float), ConvertPrimitivesDeserializer<float>.Instance },
                { typeof(double), ConvertPrimitivesDeserializer<double>.Instance },
                { typeof(decimal), ConvertPrimitivesDeserializer<decimal>.Instance },
                { typeof(DateTime), ConvertPrimitivesDeserializer<DateTime>.Instance },
                { typeof(TimeSpan), TimeSpanDeserializer.Instance },
                { typeof(bool), BooleanDeserializer.Instance },
                { typeof(Guid), GuidDeserializer.Instance },
                { typeof(Uri), UriDeserializer.Instance },
                { typeof(IPAddress), IPAddressDeserializer.Instance },
            };
    }

    internal static class OptionValueDeserializer<T> {
        private static readonly Lazy<IOptionValueDeserializer> @default;
        public static IOptionValueDeserializer Default {
            get { return @default.Value; }
        }

        static OptionValueDeserializer() {
            @default = new Lazy<IOptionValueDeserializer>(CreateDeserializer);
        }

        private static IOptionValueDeserializer CreateDeserializer() {
            // ignore nullable
            var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            IOptionValueDeserializer result;
            if (OptionValueDeserializer.PrimitiveDeserializers.TryGetValue(type, out result)) {
                return result;
            } else if (type.IsEnum) {
                return new EnumDeserializer(type);
            } else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)) {
                return KeyValuePairDeserializer.ForType(type);
            } else if (type.IsArray) {
                return ArrayDeserializer.ForType(type.GetElementType());
            } else if (type.Implements(typeof(ICollection<>))) {
                return GenericCollectionDeserializer.ForType(type);
            } else if (type.Implements(typeof(IList))) {
                return LegacyCollectionDeserializer.ForType(type);
            } else {
                throw new NotSupportedException(
                    "The option property type " + type.FullName + " is not supported."
                );
            }
        }
    }

    internal abstract class SingleValueDeserializer : IOptionValueDeserializer {
        public bool AcceptsMultipleValues {
            get { return false; }
        }

        public abstract object CreateInstance();

        public abstract object Deserialize(CultureInfo parsingCulture, object instance, string value);

        public object Deserialize(CultureInfo parsingCulture, object instance, IEnumerable<string> values) {
            throw new NotSupportedException(
                "This deserializer can only handle single values"
            );
        }
    }

    internal class StringDeserializer : SingleValueDeserializer {
        public static readonly StringDeserializer Instance = new StringDeserializer();

        private StringDeserializer() { }

        public override object CreateInstance() {
            return null;
        }

        public override object Deserialize(CultureInfo parsingCulture, object instance, string value) {
            return value;
        }
    }

    internal class ConvertPrimitivesDeserializer<T> : SingleValueDeserializer {
        public static readonly ConvertPrimitivesDeserializer<T> Instance =
            new ConvertPrimitivesDeserializer<T>();

        private ConvertPrimitivesDeserializer() { }

        public override object CreateInstance() {
            return default(T);
        }

        public override object Deserialize(CultureInfo parsingCulture, object instance, string value) {
            try {
                return Convert.ChangeType(value, typeof(T), parsingCulture);
            } catch (Exception ex) {
                throw new DeserializationException(
                    value,
                    "Value is not a valid " + typeof(T).Name,
                    ex
                );
            }
        }
    }

    internal class BooleanDeserializer : SingleValueDeserializer {
        public static readonly BooleanDeserializer Instance = new BooleanDeserializer();

        private BooleanDeserializer() { }

        public override object CreateInstance() {
            return false;
        }

        private static HashSet<string> TrueValues = new HashSet<string>(
            new[] { "true", "t", "yes", "y", "1" }, StringComparer.OrdinalIgnoreCase
        );

        private static HashSet<string> FalseValues = new HashSet<string>(
            new[] { "false", "f", "no", "n", "0" }, StringComparer.OrdinalIgnoreCase
        );

        public override object Deserialize(CultureInfo parsingCulture, object instance, string value) {
            if (TrueValues.Contains(value)) {
                return true;
            } else if (FalseValues.Contains(value)) {
                return false;
            } else {
                throw new DeserializationException(value, "Value is not a valid boolean.");
            }
        }
    }

    internal class GuidDeserializer : SingleValueDeserializer {
        public static readonly GuidDeserializer Instance = new GuidDeserializer();

        private GuidDeserializer() { }

        public override object CreateInstance() {
            return new Guid();
        }

        public override object Deserialize(CultureInfo parsingCulture, object instance, string value) {
            Guid result;
            if (Guid.TryParse(value, out result)) {
                return result;
            } else {
                throw new DeserializationException(value, "Value is not a valid guid.");
            }
        }
    }

    internal class TimeSpanDeserializer : SingleValueDeserializer {
        public static readonly TimeSpanDeserializer Instance = new TimeSpanDeserializer();

        private TimeSpanDeserializer() { }

        public override object CreateInstance() {
            return default(TimeSpan);
        }

        public override object Deserialize(CultureInfo parsingCulture, object instance, string value) {
            TimeSpan result;
            if (TimeSpan.TryParse(value, parsingCulture, out result)) {
                return result;
            } else {
                throw new DeserializationException(value, "Value is not a valid TimeSpan");
            }
        }
    }

    internal class UriDeserializer : SingleValueDeserializer {
        public static readonly UriDeserializer Instance = new UriDeserializer();

        private UriDeserializer() { }

        public override object CreateInstance() {
            return null;
        }

        public override object Deserialize(CultureInfo parsingCulture, object instance, string value) {
            try {
                return new Uri(value);
            } catch (Exception ex) {
                throw new DeserializationException(
                    value,
                    "Value is not a valid Uri",
                    ex
                );
            }
        }
    }

    internal class IPAddressDeserializer : SingleValueDeserializer {
        public static readonly IPAddressDeserializer Instance = new IPAddressDeserializer();

        private IPAddressDeserializer() { }

        public override object CreateInstance() {
            return new IPAddress(new byte[] { 0, 0, 0, 0 });
        }

        public override object Deserialize(CultureInfo parsingCulture, object instance, string value) {
            IPAddress result;
            if (IPAddress.TryParse(value, out result)) {
                return result;
            } else {
                throw new DeserializationException(value, "Value is not a valid IPAddress");
            }
        }
    }

    internal class EnumDeserializer : SingleValueDeserializer {
        private Type enumType;

        public EnumDeserializer(Type enumType) {
            this.enumType = enumType;
        }

        public override object CreateInstance() {
            return Activator.CreateInstance(enumType);
        }

        public override object Deserialize(CultureInfo parsingCulture, object instance, string value) {
            try {
                return Enum.Parse(this.enumType, value, ignoreCase: true);
            } catch (Exception ex) {
                throw new DeserializationException(
                    value,
                    "Value is not a valid " + this.enumType.Name,
                    ex
                );
            }
        }
    }

    internal static class KeyValuePairDeserializer {
        public static IOptionValueDeserializer ForType(Type keyValuePairType) {
            var types = keyValuePairType.GetGenericArguments();
            return ForType(types[0], types[1]);
        }

        public static IOptionValueDeserializer ForType(Type keyType, Type valueType) {
            return (IOptionValueDeserializer)
                ForTypeMethod.MakeGenericMethod(keyType, valueType).Invoke(null, new object[0]);
        }

        private static readonly MethodInfo ForTypeMethod =
            ReflectionHelper.GetGenericMethodDefinition(() => ForType<object, object>());

        public static KeyValuePairDeserializer<TKey, TValue> ForType<TKey,TValue>() {
            return KeyValuePairDeserializer<TKey, TValue>.Instance;
        }
    }

    internal class KeyValuePairDeserializer<TKey,TValue> : SingleValueDeserializer {
        private IOptionValueDeserializer keyDeserializer;
        private IOptionValueDeserializer valueDeserializer;

        public static readonly KeyValuePairDeserializer<TKey, TValue> Instance =
            new KeyValuePairDeserializer<TKey, TValue>();

        private KeyValuePairDeserializer() {
            this.keyDeserializer = OptionValueDeserializer<TKey>.Default;
            this.valueDeserializer = OptionValueDeserializer<TValue>.Default;
        }

        public override object CreateInstance() {
            return new KeyValuePair<TKey, TValue>();
        }

        public override object Deserialize(CultureInfo parsingCulture, object instance, string value) {
            var parts = value.Split(new[] { '=', ':' });
            if (parts.Length != 2) {
                throw new DeserializationException(
                    value,
                    "This value is not in the appropriate format for KeyValuePairs. The format " +
                    "must be key=value or key:value and neither key nor value may contain a " +
                    "colon or equal sign."
                );
            }
            return new KeyValuePair<TKey, TValue>(
                (TKey)this.keyDeserializer.Deserialize(parsingCulture, this.keyDeserializer.CreateInstance(), parts[0]),
                (TValue)this.valueDeserializer.Deserialize(parsingCulture, this.valueDeserializer.CreateInstance(), parts[1])
            );
        }
    }

    internal abstract class MultiValueDeserializer<T> : IOptionValueDeserializer {
        private IOptionValueDeserializer elementDeserializer;

        public bool AcceptsMultipleValues {
            get { return true; }
        }

        protected MultiValueDeserializer(IOptionValueDeserializer elementDeserializer) {
            if (elementDeserializer.AcceptsMultipleValues) {
                throw new InvalidOperationException(
                    "Option properties cannot use nested collection types."
                );
            }
            this.elementDeserializer = elementDeserializer;
        }

        public object Deserialize(CultureInfo parsingCulture, object instance, string value) {
            throw new NotSupportedException();
        }

        public object Deserialize(CultureInfo parsingCulture, object instance, IEnumerable<string> values) {
            var acc = CreateAccumulator(instance);
            foreach (var value in values) {
                var deserialized =
                    this.elementDeserializer.Deserialize(parsingCulture, instance, value);
                acc = OnElementDeserialized(acc, deserialized);
            }
            return FinalizeAccumulator(acc);
        }

        public abstract object CreateInstance();

        protected abstract T CreateAccumulator(object instance);
        protected abstract T OnElementDeserialized(T accumulator, object element);
        protected abstract object FinalizeAccumulator(T accumulator);
    }

    internal static class ArrayDeserializer {
        public static IOptionValueDeserializer ForType(Type elementType) {
            return (IOptionValueDeserializer)
                ForTypeMethod.MakeGenericMethod(elementType).Invoke(null, new object[0]);
        }

        private static readonly MethodInfo ForTypeMethod =
            ReflectionHelper.GetGenericMethodDefinition(() => ForType<object>());

        public static ArrayDeserializer<T> ForType<T>() {
            return ArrayDeserializer<T>.Instance;
        }
    }

    internal class ArrayDeserializer<T> : MultiValueDeserializer<IEnumerable<T>> {
        public static readonly ArrayDeserializer<T> Instance = new ArrayDeserializer<T>();

        private ArrayDeserializer()
            : base(OptionValueDeserializer<T>.Default) {
        }

        public override object CreateInstance() {
            return new T[0];
        }

        protected override IEnumerable<T> CreateAccumulator(object instance) {
            return (T[])instance;
        }

        protected override IEnumerable<T> OnElementDeserialized(IEnumerable<T> accumulator, object element) {
            return accumulator.Append((T)element);
        }

        protected override object FinalizeAccumulator(IEnumerable<T> accumulator) {
            return accumulator.ToArray();
        }
    }

    internal static class GenericCollectionDeserializer {
        private static readonly IDictionary<Type, MethodInfo> StandardCollectionFactories;

        static GenericCollectionDeserializer() {
            StandardCollectionFactories =
                new Dictionary<Type, MethodInfo> {
                    { typeof(ICollection<>), CreateListMethod },
                    { typeof(IList<>), CreateListMethod },
                    { typeof(ISet<>), CreateSetMethod },
                    { typeof(IDictionary<,>), CreateDictionaryMethod },
                };
        }

        public static IOptionValueDeserializer ForType(Type type) {
            var elementType = type.GetInterface(typeof(ICollection<>)).GetGenericArguments().Single();
            return (IOptionValueDeserializer)ForTypeMethod.MakeGenericMethod(elementType, type).Invoke(null, new object[0]);
        }

        private static readonly MethodInfo ForTypeMethod =
            ReflectionHelper.GetGenericMethodDefinition(() => ForType<object, List<object>>());
        public static GenericCollectionDeserializer<TElem, TColl> ForType<TElem, TColl>() where TColl : ICollection<TElem> {
            return GenericCollectionDeserializer<TElem, TColl>.Instance;
        }

        private static MethodInfo CreateListMethod =
            ReflectionHelper.GetGenericMethodDefinition(() => CreateList<object>());
        private static IList<T> CreateList<T>() {
            return new List<T>();
        }

        private static MethodInfo CreateSetMethod =
            ReflectionHelper.GetGenericMethodDefinition(() => CreateSet<object>());
        private static ISet<T> CreateSet<T>() {
            return new HashSet<T>();
        }

        private static MethodInfo CreateDictionaryMethod =
            ReflectionHelper.GetGenericMethodDefinition(() => CreateDictionary<object, object>());
        private static IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>() {
            return new Dictionary<TKey, TValue>();
        }

        internal static object CreateInstanceOf(Type type) {
            MethodInfo factoryMethod;
            if (StandardCollectionFactories.TryGetValue(type.GetGenericTypeDefinition(), out factoryMethod)) {
                return factoryMethod.MakeGenericMethod(type.GetGenericArguments()).Invoke(null, new object[0]);
            } else {
                throw new NotSupportedException(
                    "The generic collection interface type " + type.FullName + " is not supported."
                );
            }
        }
    }

    internal class GenericCollectionDeserializer<TElem, TColl> : MultiValueDeserializer<TColl>
        where TColl : ICollection<TElem> {

        public static readonly GenericCollectionDeserializer<TElem, TColl> Instance =
            new GenericCollectionDeserializer<TElem, TColl>();

        private GenericCollectionDeserializer()
            : base(OptionValueDeserializer<TElem>.Default) {
        }

        public override object CreateInstance() {
            if (typeof(TColl).IsInterface) {
                return GenericCollectionDeserializer.CreateInstanceOf(typeof(TColl));
            } else {
                return Activator.CreateInstance(typeof(TColl));
            }
        }

        protected override TColl CreateAccumulator(object instance) {
            return (TColl)instance;
        }

        protected override TColl OnElementDeserialized(TColl accumulator, object element) {
            accumulator.Add((TElem)element);
            return accumulator;
        }

        protected override object FinalizeAccumulator(TColl accumulator) {
            return accumulator;
        }
    }

    internal static class LegacyCollectionDeserializer {
        public static IOptionValueDeserializer ForType(Type type) {
            return (IOptionValueDeserializer)ForTypeMethod.MakeGenericMethod(type).Invoke(null, new object[0]);
        }

        private static readonly MethodInfo ForTypeMethod =
            ReflectionHelper.GetGenericMethodDefinition(() => ForType<ArrayList>());
        public static LegacyCollectionDeserializer<TColl> ForType<TColl>() where TColl : IList {
            return LegacyCollectionDeserializer<TColl>.Instance;
        }
    }

    internal class LegacyCollectionDeserializer<TColl> : MultiValueDeserializer<TColl>
        where TColl : IList {

        public static readonly LegacyCollectionDeserializer<TColl> Instance =
            new LegacyCollectionDeserializer<TColl>();

        private LegacyCollectionDeserializer()
            : base(StringDeserializer.Instance) {
        }

        public override object CreateInstance() {
            if (typeof(TColl).IsInterface) {
                if (typeof(TColl) == typeof(IList)) {
                    return new ArrayList();
                } else {
                    throw new NotSupportedException(
                        "The collection interface type " + typeof(TColl).FullName + " is not supported."
                    );
                }
            } else {
                return Activator.CreateInstance<TColl>();
            }
        }

        protected override TColl CreateAccumulator(object instance) {
            return (TColl)instance;
        }

        protected override TColl OnElementDeserialized(TColl accumulator, object element) {
            accumulator.Add(element);
            return accumulator;
        }

        protected override object FinalizeAccumulator(TColl accumulator) {
            return accumulator;
        }
    }
}
