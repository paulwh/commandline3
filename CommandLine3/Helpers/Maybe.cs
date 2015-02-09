using System;

namespace CommandLine.Helpers {
    internal struct Maybe<T> {
        public static readonly Maybe<T> Nothing = new Maybe<T>();

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

        public override int GetHashCode() {
            return this.hasValue ? this._value.GetHashCode() : 0;
        }

        public override bool Equals(object obj) {
            if (obj is Maybe<T>) {
                var other = (Maybe<T>)obj;
                if (this.hasValue) {
                    return other.hasValue && this.Value.Equals(other._value);
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

        public static implicit operator Maybe<T>(T value) {
            return value == null ? Maybe<T>.Nothing : new Maybe<T>(value);
        }
    }
}
