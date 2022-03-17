using CyberHacker.ExtensionClasses;
using System;
using System.Drawing;

namespace CyberHacker.ComparableClasses
{
    internal sealed class StructComparable : Rectangable, IToleranceComparable
    {
        internal string String = ((byte)0x00).ToHexString();
        internal new IntComparable X => Rectangle.X;
        internal new IntComparable Y => Rectangle.Y;
        internal StructComparable(in Rectangle Rectangle) => this.Rectangle = Rectangle;
        internal StructComparable(in Rectangle Rectangle, in string String) : this(Rectangle) => this.String = String;
        public bool Compare(object a, int tolerance) => this - (StructComparable)a < (tolerance * 2) &&
                                                        ((StructComparable)a).String == String;
        public static int operator -(in StructComparable a, in StructComparable b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
}
