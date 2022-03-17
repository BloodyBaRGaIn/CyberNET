using System.Drawing;

namespace CyberHacker
{
    /// <summary>
    /// Provides easier access to the rectangle properties
    /// </summary>
    abstract class Rectangable
    {
        internal Rectangle Rectangle;
        internal int X
        {
            get => Rectangle.X;
            set => Rectangle.X = value;
        }
        internal int Y
        {
            get => Rectangle.Y;
            set => Rectangle.Y = value;
        }
        internal Point Location
        {
            get => Rectangle.Location;
            set => Rectangle.Location = value;
        }
        internal int Width
        {
            get => Rectangle.Width;
            set => Rectangle.Width = value;
        }
        internal int Height
        {
            get => Rectangle.Height;
            set => Rectangle.Height = value;
        }
        internal Size Size
        {
            get => Rectangle.Size;
            set => Rectangle.Size = value;
        }

        public override bool Equals(object obj) =>
            obj is Rectangable rectangable &&
            Location == rectangable.Location &&
            Size == rectangable.Size;

        public override int GetHashCode()
        {
            return Rectangle.GetHashCode();
        }
    }
}
