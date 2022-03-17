using System;

namespace CyberHacker.ComparableClasses
{
    internal sealed class IntComparable : IToleranceComparable, IComparable
    {
        internal int Value;

        internal IntComparable(int Value) => this.Value = Value;

        public bool Compare(object obj, int tolerance) => Math.Abs((IntComparable)obj - this) < tolerance;

        public static explicit operator int(IntComparable comparable) => comparable.Value;

        public static implicit operator IntComparable(int val) => new(val);

        public static int operator -(IntComparable a, IntComparable b) => (int)a - (int)b;

        public static bool operator ==(IntComparable a, IntComparable b) => (int)a == (int)b;

        public static bool operator !=(IntComparable a, IntComparable b) => (int)a != (int)b;

        public override bool Equals(object obj) => obj is IntComparable comparable && this == comparable;

        public override int GetHashCode() => Value.GetHashCode();

        public int CompareTo(object obj) => obj switch
        {
            IntComparable comparable => Value.CompareTo((int)comparable),
            _ => throw new ArgumentException(nameof(obj))
        };
    }
}
