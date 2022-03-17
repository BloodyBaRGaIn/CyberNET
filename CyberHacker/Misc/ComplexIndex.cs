using System;

namespace CyberHacker
{
    internal struct ComplexIndex
    {
        internal readonly int X;
        internal readonly int Y;

        internal ComplexIndex(in int X = 0, in int Y = 0)
        {
            this.X = X;
            this.Y = Y;
        }

        internal static int Distance(ComplexIndex current, ComplexIndex next) =>
            Math.Abs(next.X - current.X) + Math.Abs(next.Y - current.Y);

        public override bool Equals(object obj) =>
            obj is ComplexIndex other
            && X == other.X
            && Y == other.Y;

        public void Deconstruct(out int X, out int Y)
        {
            X = this.X;
            Y = this.Y;
        }

        public static implicit operator (int, int)(ComplexIndex value)
        {
            return (value.X, value.Y);
        }

        public static implicit operator ComplexIndex((int X, int Y) value)
        {
            return new ComplexIndex(value.X, value.Y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
