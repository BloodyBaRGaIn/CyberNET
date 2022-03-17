using CyberHacker.ComparableClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CyberHacker.ExtensionClasses
{
    internal static class BitmapExtensions
    {
        /// <summary>
        /// A screen capture method
        /// </summary>
        /// <returns>A <see cref="Bitmap"/> object representing a screenshot</returns>
        internal static Bitmap Capture()
        {
            Rectangle bounds = System.Windows.Forms.Screen.GetBounds(Point.Empty);
            Bitmap bitmap = new(bounds.Width, bounds.Height);
            Graphics.FromImage(bitmap).CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            return bitmap;
        }

        /// <summary>
        /// Searches an image into another one by resource name
        /// </summary>
        /// <param name="sourceBitmap">An image to search into</param>
        /// <param name="namespace">A current namespace</param>
        /// <param name="resourceprefix">The resource first name</param>
        /// <param name="tolerance">A value used for comparing bytes. Zero value is for finding a perfect matches</param>
        /// <returns>Coordinates of matches found</returns>
        internal static List<Rectangle> FindBitmapsEntry(this Bitmap sourceBitmap, in string @namespace, in string resourceprefix,
                                                         in byte tolerance = 0)
        {
            List<Rectangle> res = new();
            foreach (Bitmap bitmap in ResourceBitmaps(@namespace, resourceprefix))
            {
                res.AddRange(FindBitmapsEntry(sourceBitmap, bitmap, tolerance));
            }
            return res;
        }

        /// <summary>
        /// Searches an image into another one
        /// </summary>
        /// <param name="sourceBitmap">An image to search into</param>
        /// <param name="serchingBitmap">A desired image</param>
        /// <param name="tolerance">A value used for comparing bytes. Zero value is for finding a perfect matches</param>
        /// <returns>Coordinates of matches found</returns>
        private static List<Rectangle> FindBitmapsEntry(in Bitmap sourceBitmap,
                                                        in Bitmap serchingBitmap,
                                                        byte tolerance = 0)
        {
            if (sourceBitmap.PixelFormat != serchingBitmap.PixelFormat)
            {
                throw new ArgumentException("PixelFormat");
            }
            int pixelFormatSize = Image.GetPixelFormatSize(sourceBitmap.PixelFormat) / 8;
            sourceBitmap.PrepareImage(out byte[] sourceBytes, out BitmapData sourceBitmapData);
            serchingBitmap.PrepareImage(out byte[] serchingBytes, out BitmapData serchingBitmapData);
            List<Rectangle> pointsList = new();
            bool LessThanTolerance(in byte a, in byte b)
            {
                return Difference(a, b) < tolerance;
            }
            ulong Difference(in byte a, in byte b)
            {
                return (ulong)Math.Abs(a - b);
            }
            for (int mainY = 0; mainY < sourceBitmap.Height - serchingBitmap.Height + 1; mainY++)
            {
                int sourceY = mainY * sourceBitmapData.Stride;

                for (int mainX = 0; mainX < sourceBitmap.Width - serchingBitmap.Width + 1; mainX++)
                {
                    int sourceX = mainX * pixelFormatSize;
                    bool isEqual = true;
                    for (int c = 0; c < pixelFormatSize; c++)
                    {
                        if (!LessThanTolerance(sourceBytes[sourceX + sourceY + c],
                            serchingBytes[c]))
                        {
                            isEqual = false;
                            break;
                        }
                    }
                    if (!isEqual)
                    {
                        continue;
                    }
                    bool isStop = false;
                    for (int secY = 0; secY < serchingBitmap.Height; secY++)
                    {
                        int serchY = secY * serchingBitmapData.Stride;
                        int sourceSecY = (mainY + secY) * sourceBitmapData.Stride;
                        for (int secX = 0; secX < serchingBitmap.Width; secX++)
                        {
                            int serchX = secX * pixelFormatSize;
                            int sourceSecX = (mainX + secX) * pixelFormatSize;
                            for (int c = 0; c < pixelFormatSize; c++)
                            {
                                if (!LessThanTolerance(sourceBytes[sourceSecX + sourceSecY + c],
                                                      serchingBytes[serchX + serchY + c]))
                                {
                                    isStop = true;
                                    break;
                                }
                            }
                            if (isStop)
                            {
                                break;
                            }
                        }
                        if (isStop)
                        {
                            break;
                        }
                    }
                    if (!isStop)
                    {
                        pointsList.Add(new Rectangle(new Point(mainX, mainY), serchingBitmap.Size));
                    }
                }
            }
            return pointsList;
        }

        /// <summary>
        /// Gets an advanced data from the image
        /// </summary>
        /// <param name="bitmap">The given image</param>
        /// <param name="data">The bytes of image data scan</param>
        /// <param name="bitmapData">The image data providing object</param>
        private static void PrepareImage(this Bitmap bitmap, out byte[] data, out BitmapData bitmapData)
        {
            bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int BitmapBytesLength = bitmapData.Stride * bitmapData.Height;
            data = new byte[BitmapBytesLength];
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, data, 0, BitmapBytesLength);
            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// Gets the points to click
        /// </summary>
        /// <param name="rects">Target regions</param>
        /// <param name="shift">A shift from the upper-left corner</param>
        /// <param name="points">An output collection of points</param>
        internal static void GetPoints(in IEnumerable<Rectangle> rects, in Point shift, out List<Point> points)
        {
            points = new();
            for (int i = 0; i < rects.Count(); i++)
            {
                Rectangle rectangle = Stretch(rects.ElementAt(i), 3);
                points.Add(rectangle.Location + (rectangle.Size / 2) + (Size)shift);
            }
        }

        /// <summary>
        /// Strethes the given rectangle
        /// </summary>
        /// <param name="based">The goven rectangle</param>
        /// <param name="stretch">The value to stretch (could be negative to shrink)</param>
        /// <returns>A new stretched rectangle</returns>
        private static Rectangle Stretch(Rectangle based, int stretch)
        {
            Size size = new(stretch, stretch);
            return new Rectangle(based.Location - size, based.Size + (size * 2));
        }

        /// <summary>
        /// Changes the scale of the image
        /// <para>See <seealso cref="Scale(Bitmap, float, object)"/></para>
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="scale"></param>
        internal static void Scale(ref Bitmap bitmap, in float scale = 1f) => bitmap = bitmap.Scale(scale);

        /// <summary>
        /// Changes the scale of the image
        /// </summary>
        /// <param name="bitmap">The image to scale</param>
        /// <param name="scale">The scaling coefficient</param>
        /// <param name="tag">A tag object for resulting image. If it's not given or null it will be taken from the source image object</param>
        /// <returns>A new scaled image</returns>
        private static Bitmap Scale(this Bitmap bitmap, in float scale = 1f, in object tag = null) => new(bitmap, (bitmap.Size * scale).ToSize()) { Tag = tag ?? bitmap.Tag };

        /// <summary>
        /// Provides a <see cref="Bitmap"/> objects from the resource file
        /// </summary>
        /// <param name="namespace">A current namespace</param>
        /// <param name="resourceprefix">A resource first name</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Bitmap"/> objects</returns>
        private static IEnumerable<Bitmap> ResourceBitmaps(in string @namespace, in string resourceprefix) =>
            Type.GetType($"{@namespace.FirstUpper()}.Properties.{resourceprefix.FirstUpper()}Resources")
                .GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .Where(t => t.PropertyType == typeof(Bitmap))
                .Select(item => new Bitmap((Bitmap)item.GetValue(null)) { Tag = item.Name.Trim('_') });

        /// <summary>
        /// Runs a parallel search for the images
        /// </summary>
        /// <param name="source">A source image to search into</param>
        /// <param name="namespace">A current namespace</param>
        /// <param name="resourceprefix">The resource first name</param>
        /// <param name="is_square">Indicates that the result should have an equal dimensions lengths</param>
        /// <param name="corrected">An output collection of strings</param>
        /// <param name="rectangles">An output collection of coordinates and sizes</param>
        /// <param name="Cell_coords">An output collection of tho-dimensional indexes and coordinates</param>
        /// <param name="scale">An additional scale for the source image</param>
        /// <returns>A string representing the data on the image</returns>
        internal static string BitmapRecognition(this Bitmap source, in string @namespace, in string resourceprefix, in bool is_square,
                                                 out List<string[]> corrected, out IEnumerable<Rectangle> rectangles,
                                                 out Dictionary<ComplexIndex, Rectangle> Cell_coords, float scale = 1f)
        {
            List<IToleranceComparable> result = new();
            corrected = new();
            var resources = ResourceBitmaps(@namespace, resourceprefix);
            int tolerance = resources.Select(item => item.Width + item.Height).Max(item => item) / 4;
            System.Threading.Tasks.ParallelLoopResult parfor = System.Threading.Tasks.Parallel.ForEach
            (
                from Bitmap bitmap in resources
                let src = source.Scale(scale)
                let des = bitmap.Scale(scale)
                select (src, des),
                item => result.AddRange(
                    FindBitmapsEntry(item.src, item.des, 150)
                        .Select(rect => (IToleranceComparable)(new StructComparable(rect, item.des.Tag.ToString())))));
            result = result.CuttedList(tolerance).ToList();
            rectangles = result.Select(item => (item as StructComparable).Rectangle);
            {
                IEnumerable<StructComparable> XS = rectangles.Select(item => new StructComparable(item)).CuttedList(tolerance);
                IEnumerable<IntComparable> SelX = XS.Select(item => item.X);
                IEnumerable<IntComparable> SelY = XS.Select(item => item.Y);
                for (int i = 0; i < result.Count; i++)
                {
                    Align(i, true, SelX, tolerance, ref result);
                    Align(i, false, SelY, tolerance, ref result);
                }
                Size MaxSize = result.Select(item => item as StructComparable).Max(item => (item.Width, item.Height)).StructSize();
                foreach (StructComparable @struct in result)
                {
                    @struct.Size = MaxSize;
                }
            }
            result = result.Select(item => item as StructComparable)
                           .OrderBy(item => (int)item.Y)
                           .ThenBy(item => (int)item.X)
                           .Distinct()
                           .Select(item => item as IToleranceComparable)
                           .ToList();
            Cell_coords = new();
            if (is_square)
            {
                IEnumerable<StructComparable> structs = result.Select(item => item as StructComparable);
                IOrderedEnumerable<IntComparable> XX = structs.Select(item => item.X).Distinct().OrderBy(item => item),
                                                  YY = structs.Select(item => item.Y).Distinct().OrderBy(item => item);
                int idx_row = 0;
                foreach (int y in YY)
                {
                    int idx_col = 0;
                    corrected.Add(new string[XX.Count()]);
                    foreach (int x in XX)
                    {
                        IEnumerable<StructComparable> search = structs.Where(st => st.X == x && st.Y == y);
                        StructComparable item = (search.FirstOrNull()
                            ?? new StructComparable(new Rectangle(new Point(x, y), structs.First().Size))) as StructComparable;
                        corrected[idx_row][idx_col] = item.String;
                        Cell_coords.Add((idx_row, idx_col), item.Rectangle);
                        idx_col++;
                    }
                    idx_row++;
                }
            }
            else
            {
                List<Rectangle> First_col = result.Select(item => item as StructComparable)
                                                  .Where(item => (int)item.X == result.Min(item => (int)(item as StructComparable).X))
                                                  .Select(item => item.Rectangle)
                                                  .OrderBy(item => item.Y)
                                                  .ToList();
                corrected.AddRange(from IntComparable Y in First_col.Select(item => new IntComparable(item.Y))
                                   select new string[result.Where(item => (item as StructComparable).Y == Y).Count()]);
                try
                {
                    int idx_row = 0, idx_col = 0;
                    foreach (StructComparable item in result)
                    {
                        int new_idx = First_col.IndexOf(item.Rectangle);
                        if (new_idx != -1)
                        {
                            idx_row = new_idx;
                            idx_col = 0;
                        }
                        corrected[idx_row][idx_col] = item.String;
                        Cell_coords.Add((idx_row, idx_col++), item.Rectangle);
                    }
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    throw new FormatException();
                }
            }
            rectangles = Cell_coords.Values;
            string res = "";
            foreach (string[] line in corrected)
            {
                foreach (string word in line)
                {
                    res += $"{word} ";
                }
                res += Constants.NL;
            }
            return res;
        }

        /// <summary>
        /// An alignment method
        /// </summary>
        /// <param name="i">An index in the source collection</param>
        /// <param name="is_X">Indicates that the X if frue, otherwise - the Y coordinate is aligning</param>
        /// <param name="align_list">A collection of the resulting coordinates values</param>
        /// <param name="tolerance">The tolerance for comparing. See also <seealso cref="IToleranceComparable"/>, <seealso cref="IntComparable.Compare(object, int)"/></param>
        /// <param name="result">A source collection to align</param>
        private static void Align(in int i, bool is_X, in IEnumerable<IntComparable> align_list, int tolerance, ref List<IToleranceComparable> result)
        {
            StructComparable iter = result[i] as StructComparable;
            int coord;
            {
                IEnumerable<IntComparable> find = align_list.Where(item => (is_X ? iter.X : iter.Y).Compare(item, tolerance));
                if (!find.Any())
                {
                    return;
                }
                coord = (int)(find.FirstOrDefault() as IntComparable);
            }
            iter.Location = is_X ?
                            new(coord, (int)iter.Y) :
                            new((int)iter.X, coord);
        }

        /// <summary>
        /// Clears memory of the image
        /// </summary>
        /// <param name="bitmap">A given image</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static unsafe void Clear(this Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return;
            }
            BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                                              ImageLockMode.ReadWrite,
                                              bitmap.PixelFormat);
            Fill(data.Scan0.ToPointer(), 0, bitmap.Width * bitmap.Height);
            bitmap.UnlockBits(data);
        }

        /// <summary>
        /// Clears memory by the pointer
        /// </summary>
        /// <param name="ptr">A memory pointer</param>
        /// <param name="value">The value to write</param>
        /// <param name="number">A number of bytes to fill</param>
        private static unsafe void Fill(void* ptr, in int value, in long number)
        {
            long longValue = 0;
            ((int*)&longValue)[0] = value;
            ((int*)&longValue)[1] = value;
            long length = number << 2;
            byte* dst = (byte*)ptr;
            if (((int)dst & 4) != 0)
            {
                *(int*)dst = value;
                dst += 4;
                length -= 4;
            }
            long total = length >> 5;
            while (total > 0)
            {
                ((long*)dst)[0] = longValue;
                ((long*)dst)[1] = longValue;
                ((long*)dst)[2] = longValue;
                ((long*)dst)[3] = longValue;
                dst += 32;
                --total;
            }
            if ((length & 16) != 0)
            {
                ((long*)dst)[0] = longValue;
                ((long*)dst)[1] = longValue;
                dst += 16;
            }
            if ((length & 8) != 0)
            {
                *(long*)dst = longValue;
                dst += 8;
            }
            if ((length & 4) != 0)
            {
                *(int*)dst = value;
            }
        }
    }
}
