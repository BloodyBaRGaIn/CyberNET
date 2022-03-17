using System;
using System.Drawing;

namespace CyberHacker
{
    internal sealed class CellText : Rectangable
    {
        internal readonly string Text;

        internal CellText(Rectangle Rectangle, string Text)
        {
            this.Rectangle = Rectangle;
            this.Text = Text;
        }

        public override bool Equals(object obj) =>
            obj is CellText other
            && base.Equals(obj)
            && Text == other.Text;

        public override int GetHashCode()
        {
            return HashCode.Combine(Rectangle, Text);
        }

        public void Deconstruct(out Rectangle Rectangle, out string Text)
        {
            Rectangle = this.Rectangle;
            Text = this.Text;
        }

        public static implicit operator (Rectangle Rectangle, string Text)(CellText value)
        {
            return (value.Rectangle, value.Text);
        }

        public static implicit operator CellText((Rectangle Rectangle, string Text) value)
        {
            return new CellText(value.Rectangle, value.Text);
        }
    }
}
