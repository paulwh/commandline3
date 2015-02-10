using System;

namespace CommandLine.Helpers {
    public static class Maybe {
        public static Maybe<T> Just<T>(T value) {
            return new Maybe<T>(value);
        }

        public static Maybe<T> FromReference<T>(T value) where T : class {
            return value != null ? new Maybe<T>(value) : Maybe<T>.Nothing;
        }

        public static Maybe<T> FromNullable<T>(T? value) where T : struct {
            return value.HasValue ? new Maybe<T>(value.Value) : Maybe<T>.Nothing;
        }

        public static Maybe<T> Nothing<T>() {
            return Maybe<T>.Nothing;
        }
    }

    public struct Maybe<T> {
        public static readonly Maybe<T> Nothing = default(Maybe<T>);

        private bool hasValue;
        private T _value;

        public bool HasValue {
            get {
                return this.hasValue;
            }
        }

        public T Value {
            get {
                if (!this.hasValue) {
                    throw new InvalidOperationException(
                        "Attempted to retrieve the value from a Maybe without a value."
                    );
                } else {
                    return this._value;
                }
            }
        }

        public Maybe(T value) {
            hasValue = true;
            _value = value;
        }

        public T GetValueOrDefault(T defaultValue) {
            return this.hasValue ? this.Value : defaultValue;
        }

        public override int GetHashCode() {
            return this.hasValue ? this._value.GetHashCode() : 0;
        }

        public override bool Equals(object obj) {
            if (obj is Maybe<T>) {
                var other = (Maybe<T>)obj;
                if (this.hasValue) {
                    return other.hasValue && Object.Equals(this.Value, other._value);
                } else {
                    return !other.hasValue;
                }
            } else {
                return false;
            }
        }

        public override string ToString() {
            return this.hasValue ? this._value.ToString() : String.Empty;
        }
    }
}
