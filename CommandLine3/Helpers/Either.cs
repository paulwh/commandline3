using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Helpers {
    internal enum EitherType {
        Left,
        Right
    }

    public static class EitherExtensions {
        public static bool IsLeft(this Either either) {
            return either.Type == EitherType.Left;
        }

        public static bool IsRight(this Either either) {
            return either.Type == EitherType.Right;
        }

        public static Tuple<IEnumerable<TLeft>, IEnumerable<TRight>> Partition<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> eithers) {
            return Tuple.Create(
                eithers.Lefts(),
                eithers.Rights()
            );
        }

        public static IEnumerable<TLeft> Lefts<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> eithers) {
            return eithers.Where(IsLeft).Select(e => e.FromLeft());
        }

        public static IEnumerable<TRight> Rights<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> eithers) {
            return eithers.Where(IsRight).Select(e => e.FromRight());
        }
    }

    internal abstract class Either {
        public EitherType Type { get; private set; }

        protected Either(EitherType type) {
            this.Type = type;
        }
    }

    internal class Either<TLeft, TRight> : Either {
        private TLeft left;
        private TRight right;

        public Either(TLeft left)
            : base(EitherType.Left) {

            this.left = left;
        }

        public Either(TRight right)
            : base(EitherType.Right) {

            this.right = right;
        }

        public TLeft FromLeft() {
            if (this.Type != EitherType.Left) {
                throw new InvalidOperationException("Cannot get the left value from an Either of type right");
            }
            return this.left;
        }

        public TRight FromRight() {
            if (this.Type != EitherType.Right) {
                throw new InvalidOperationException("Cannot get the right value from an Either of type left");
            }
            return this.right;
        }

        public TValue WithEither<TValue>(Func<TLeft, TValue> fromLeft, Func<TRight, TValue> fromRight) {
            switch (this.Type) {
                case EitherType.Left:
                    return fromLeft(this.left);
                case EitherType.Right:
                    return fromRight(this.right);
                default:
                    throw new NotSupportedException("Unsupported either type");
            }
        }

        public void WithEither(Action<TLeft> withLeft, Action<TRight> withRight) {
            switch (this.Type) {
                case EitherType.Left:
                    withLeft(this.left);
                    break;
                case EitherType.Right:
                    withRight(this.right);
                    break;
                default:
                    throw new NotSupportedException("Unsupported either type");
            }
        }

        public static implicit operator Either<TLeft, TRight>(TLeft left) {
            return new Either<TLeft, TRight>(left);
        }

        public static implicit operator Either<TLeft, TRight>(TRight right) {
            return new Either<TLeft, TRight>(right);
        }
    }
}
