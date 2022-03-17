using CyberHacker.ExtensionClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace CyberHacker.BitmapProcClasses
{
    internal static class CustomBitmap
    {
        #region Colors constants
        private static readonly Color clgreen = Color.FromArgb(218, 252, 92);
        private static readonly Color clcyan = Color.FromArgb(95, 247, 255);
        private static readonly Color clback = Color.FromArgb(18, 14, 25);
        private static readonly Color clwhite = Color.FromArgb(244, 244, 245);
        private static readonly Color clgray = Color.FromArgb(63, 60, 56);
        #endregion
        /// <summary>
        /// Border width in pixels
        /// <para>Even numbers are recommended</para>
        /// </summary>
        private const uint THICK = 2;
        private static readonly string[] IEnumTypes = new string[1] { "Rectangle" };
        private static readonly string[] DictionaryTypes = new string[2] { "ComplexIndex", "CellText" };

        /// <summary>
        /// An array of the actions
        /// </summary>
        private static readonly Action<object[]>[] actions = new Action<object[]>[3]
        {
            //MatrixAction
            new((args) =>
            {
                args.Deconstruct<Dictionary<ComplexIndex, CellText>, IEnumerable<ComplexIndex>>(
                    out var Info, out var Size, out var Graphics, out var Arg);
                int max = Info.Max(item => Math.Max(item.Value.Width, item.Value.Height)) * 4 / 3;
                Size maxsize = new(max, max);
                using SolidBrush backbrush = new(clback);
                using Pen cyanpen = new(clcyan, 2);
                int arg_cnt = Arg.Count();
                Rectangle Targ(int idx) => new(Info[Arg.ElementAt(idx)].Location - Size, maxsize);
                #region Arrow drawing
                for (int i = 0; i < arg_cnt - 1; i++)
                {
                    (Point f, Point s, Size arr) = ArrowDirection.GetDir(Targ(i), Targ(i + 1));
                    Graphics.DrawLine(cyanpen, f, s);
                    Point[] points = new Point[]
                    {
                        f,
                        f - new Size(arr.Height, arr.Height),
                        f + new Size(arr.Height, -arr.Height),
                        f - new Size(arr.Width, arr.Width),
                        f + new Size(-arr.Width, arr.Width)
                    };
                    Graphics.FillPolygon(new SolidBrush(clcyan), points.Distinct().Select(p => p + arr).ToArray());
                }
                #endregion
                #region Selected elements highlight
                for (int i = 0; i < arg_cnt; i++)
                {
                    Rectangle targ = Targ(i);
                    Graphics.FillRectangle(backbrush, targ);
                    Graphics.DrawRectangle(cyanpen, targ);
                }
                #endregion
                using Font textfont = new(Constants.font.FontFamily, 20);
                using SolidBrush textbrush = new(clgreen);
                foreach (var item in Info)
                {
                    Graphics.DrawString(item.Value.Text, textfont, textbrush, item.Value.Location - Size);
                }
            }),
            //SequenceAction
            new((args) =>
            {
                args.Deconstruct<Dictionary<ComplexIndex, CellText>, List<(bool comp, int)>>(
                    out var Info, out var Size, out var Graphics, out var Arg);
                using Font textfont = new(Constants.font.FontFamily, 20);
                for (int i = 0; i < Arg.Count; i++)
                {
                    foreach (var item in Info.Where(t => t.Key.X == i))
                    {
                        Graphics.DrawString(item.Value.Text, textfont, new SolidBrush(Arg[i].comp ? clwhite : clgray),
                                            item.Value.Location - Size);
                    }
                }
            }),
            //BufferAction
            new((args) =>
            {
                args.Deconstruct<IEnumerable<Rectangle>, string[]>(out var Info, out var Size, out var Graphics, out var Arg);
                using Font textfont = new(Constants.font.FontFamily, 16);
                for (int i = 0; i < Info.Count(); i++)
                {
                    Rectangle rectangle = Info.ElementAt(i);
                    Rectangle targ = new(rectangle.Location - Size, rectangle.Size);
                    bool fill_words = Arg.Length > i;
                    Graphics.DrawRectangle(new Pen(fill_words ? clcyan : clgreen, 2), targ);
                    if (fill_words)
                    {
                        Graphics.DrawString(Arg[i], textfont, new SolidBrush(clcyan),
                                            targ.Location - new Size(1 + (Arg[i] == "1C" ? 1 : 0), 1));
                    }
                }
            })
        };

        /// <summary>
        /// The action invocation wrapper
        /// </summary>
        /// <param name="info"></param>
        /// <param name="format"></param>
        /// <param name="action"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static Bitmap Make(Dictionary<ComplexIndex, CellText> info, PixelFormat format,
                                   Action<object[]> action, object arg)
        {
            action.Invoke(PrepArgs(info, info.Select(item => item.Value.Rectangle), 3, format, arg, out Bitmap result));
            return result;
        }

        /// <summary>
        /// The action invocation wrapper
        /// </summary>
        /// <param name="info"></param>
        /// <param name="format"></param>
        /// <param name="action"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static Bitmap Make(IEnumerable<Rectangle> info, PixelFormat format,
                                   Action<object[]> action, object arg)
        {
            action.Invoke(PrepArgs(info, info, 2, format, arg, out Bitmap result));
            return result;
        }

        /// <summary>
        /// Returns an arguments for the action as an array of objects instances
        /// </summary>
        /// <param name="origin">The main information object instance</param>
        /// <param name="rectangles">A collection of elements rectangles</param>
        /// <param name="scale">A margin scale</param>
        /// <param name="format">A pixel format</param>
        /// <param name="arg">An additional argument for the action</param>
        /// <param name="bitmap">A prepared image</param>
        /// <returns>An array of objects instances</returns>
        private static object[] PrepArgs(object origin, IEnumerable<Rectangle> rectangles, int scale, PixelFormat format,
                                         object arg, out Bitmap bitmap)
        {
            Size element_size = rectangles.Max(item => (item.Width, item.Height)).StructSize();
            Size shift = (Size)rectangles.First().Location - element_size;
            Size size = (Size)(rectangles.Last().Location + element_size * scale) - shift;
            PrepBitmap(size, format, out bitmap, out Graphics graphics);
            return new object[] { origin, shift, graphics, arg };
        }

        /// <summary>
        /// Makes a blank image filled with back color and bordered with green color
        /// </summary>
        /// <param name="size">The size of the resulting image</param>
        /// <param name="format">The pixel format</param>
        /// <param name="bitmap">The resulting image</param>
        /// <param name="graphics">The <see cref="Graphics"/> object instnce based on the image</param>
        private static void PrepBitmap(Size size, PixelFormat format, out Bitmap bitmap, out Graphics graphics)
        {
            bitmap = new(size.Width, size.Height, format);
            Rectangle border = new(Point.Empty, size);
            graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(new SolidBrush(clback), border);
            graphics.DrawRectangle(new Pen(clgreen, THICK), border);
        }

        /// <summary>
        /// Gets all the generic types of the given type
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="vs">An output collection of a generic type names</param>
        private static void GetTypenames(Type type, ref List<string> vs)
        {
            Type[] args = type.GetGenericArguments();
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    GetTypenames(args[i], ref vs);
                }
            }
            else
            {
                vs.Add(type.Name);
            }
        }

        /// <summary>
        /// Type convertion wrapper
        /// </summary>
        /// <param name="info">An instance of an object providing information</param>
        /// <param name="format">A pixel format of the image</param>
        /// <param name="action">An action to perform on the image</param>
        /// <param name="arg">Additional argument for the action</param>
        /// <returns>A custom image</returns>
        private static Bitmap MakeByObjects(object info, PixelFormat format, Action<object[]> action, object arg)
        {
            List<string> typenames = new();
            GetTypenames(info.GetType(), ref typenames);
            if (typenames.SequenceEqual(IEnumTypes))
            {
                return Make((IEnumerable<Rectangle>)info, format, action, arg);
            }
            if (typenames.SequenceEqual(DictionaryTypes))
            {
                return Make((Dictionary<ComplexIndex, CellText>)info, format, action, arg);
            }
            return null;
        }

        /// <summary>
        /// Makes a full competed custom image with all the information on it
        /// </summary>
        /// <param name="infos">Array of information object instances: matrix, sequences, and buffer information</param>
        /// <param name="formats">A pixel formats array. All elements must be the same</param>
        /// <param name="args">An additional arguments for each of the actions</param>
        /// <returns>A full competed custom image</returns>
        internal static Bitmap MakeFinal(object[] infos, PixelFormat[] formats, object[] args)
        {
            if (infos is null)
            {
                throw new ArgumentNullException(nameof(infos));
            }
            if (formats is null)
            {
                throw new ArgumentNullException(nameof(formats));
            }
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            if ((new int[3] { infos.Length, formats.Length, args.Length }).DistinctCount() > 1)
            {
                throw new ArgumentException("Input arrays are of different length");
            }
            if (args.Length > actions.Length)
            {
                throw new ArgumentException($"Arrays of length {actions.Length} were expected");
            }
            if (formats.DistinctCount() > 1)
            {
                throw new ArgumentException("Pixel formats differ");
            }
            Bitmap[] bitmaps = new Bitmap[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                bitmaps[i] = MakeByObjects(infos[i], formats[i], actions[i], args[i]);
            }
            using (Bitmap Matrix = bitmaps[0],
                          Sequence = bitmaps[1],
                          Buffer = bitmaps[2])
            {
                int half = (int)(THICK / 2);
                PrepBitmap(new(Matrix.Width + Math.Max(Sequence.Width, Buffer.Width) - half,
                               Math.Max(Matrix.Height, Sequence.Height + Buffer.Height)),
                           Matrix.PixelFormat, out Bitmap ress, out Graphics graphics);
                graphics.DrawImage(Matrix, Point.Empty);
                graphics.DrawImage(Buffer, new Point(Matrix.Width - half, 0));
                graphics.DrawImage(Sequence, new Point(Matrix.Width - half, Buffer.Height - half));
                return ress;
            }
        }
    }
}
