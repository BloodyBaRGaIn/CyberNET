using System.Drawing;

namespace CyberHacker.BitmapProcClasses
{
    internal static class ArrowDirection
    {
        /// <summary>
        /// Borders sides representing enumerable
        /// </summary>
        private enum Borders
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3,
            Center = 4
        }

        /// <summary>
        /// Gets two points between tho rectangles to draw the line between them and the shift size to draw a triangle arrow
        /// </summary>
        /// <param name="first">The first rectangle. The arrow will be based on one of the sides</param>
        /// <param name="second">The second rectangle</param>
        /// <returns>Two points and shift size</returns>
        internal static (Point, Point, Size) GetDir(Rectangle first, Rectangle second)
        {
            (Point, Point, Size) Get(Borders b1, Borders b2) => (first.MiddlePoint(b1, out Size arr), second.MiddlePoint(b2, out _), arr);
            if (first.X - second.X > 0)
            {
                return Get(Borders.Left, Borders.Right);
            }
            if (first.X - second.X < 0)
            {
                return Get(Borders.Right, Borders.Left);
            }
            if (first.Y - second.Y > 0)
            {
                return Get(Borders.Top, Borders.Bottom);
            }
            if (first.Y - second.Y < 0)
            {
                return Get(Borders.Bottom, Borders.Top);
            }
            return (Point.Empty, Point.Empty, new());
        }

        /// <summary>
        /// Gets the point on the middle of given border of the rectangle
        /// </summary>
        /// <param name="rectangle">The rectangle</param>
        /// <param name="borders">The side of the rectangle</param>
        /// <param name="arrow">A shift that gives the direction of the arrow</param>
        /// <returns></returns>
        private static Point MiddlePoint(this Rectangle rectangle, Borders borders, out Size arrow)
        {
            int shift = 10;
            arrow = new();
            Point res = rectangle.Location;
            switch (borders)
            {
                case Borders.Left:
                    res += new Size(0, rectangle.Height / 2);
                    arrow = new(-shift, 0);
                    break;
                case Borders.Top:
                    res += new Size(rectangle.Width / 2, 0);
                    arrow = new(0, -shift);
                    break;
                case Borders.Right:
                    res += new Size(rectangle.Width, rectangle.Height / 2);
                    arrow = new(shift, 0);
                    break;
                case Borders.Bottom:
                    res += new Size(rectangle.Width / 2, rectangle.Height);
                    arrow = new(0, shift);
                    break;
                case Borders.Center:
                    res += new Size(rectangle.Width / 2, rectangle.Height / 2);
                    break;
                default:
                    break;
            };
            return res;
        }
    }
}
