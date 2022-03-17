using CyberHacker.ExtensionClasses;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CyberHacker.BitmapProcClasses
{
    /// <summary>
    /// Providing a complex information referenced to the image
    /// </summary>
    internal sealed class ParsedBitmap
    {
        internal readonly string Result;
        internal readonly List<string[]> Parsed;
        private IEnumerable<Rectangle> cells;
        private IEnumerable<ComplexIndex> solution_path;
        internal IEnumerable<Rectangle> Cells => cells;
        internal IEnumerable<ComplexIndex> Solution_path => solution_path;
        internal readonly Dictionary<ComplexIndex, Rectangle> Coords;

        internal Dictionary<ComplexIndex, CellText> GetFullInfo()
        {
            Dictionary<ComplexIndex, CellText> info = new();
            foreach (var key in Coords.Keys)
            {
                info.Add(key, (Coords[key], Parsed[key.X][key.Y]));
            }
            return info;
        }

        internal ParsedBitmap(ref Bitmap bitmap, in string name, in string folder, in bool is_sq, in float scale = 1) =>
            PrepResult(ref Result, ref bitmap, name, folder, is_sq, out Parsed, out cells, out Coords, scale);

        internal ParsedBitmap(ref Bitmap bitmap, in string name, in string folder)
        {
            cells = bitmap.FindBitmapsEntry(name, folder, 70);
            Result = cells.Count().ToString();
        }

        internal void SetSolutionPath(IEnumerable<ComplexIndex> ps)
        {
            solution_path = ps.ToList();
        }

        internal IEnumerable<string> KeyText => solution_path.Select(key => Parsed[key.X][key.Y]);
        internal IEnumerable<Rectangle> KeyCoords => solution_path.Select(key => Coords[key]);

        internal void AddCell(in Rectangle value) => cells = cells.Add(value);

        /// <summary>
        /// Image search wrapper
        /// <para>See <see cref="BitmapExtensions.BitmapRecognition(Bitmap, in string, in string, in bool, out List{string[]}, out IEnumerable{Rectangle}, out Dictionary{ComplexIndex, Rectangle}, float)"/></para>
        /// </summary>
        /// <param name="result"></param>
        /// <param name="bitmap"></param>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <param name="is_sq"></param>
        /// <param name="parsed"></param>
        /// <param name="cells"></param>
        /// <param name="coords"></param>
        /// <param name="scale"></param>
        private static void PrepResult(ref string result, ref Bitmap bitmap, in string name, in string folder, in bool is_sq,
                                       out List<string[]> parsed, out IEnumerable<Rectangle> cells,
                                       out Dictionary<ComplexIndex, Rectangle> coords, in float scale = 1)
        {
            while (true)
            {
                try
                {
                    result = bitmap.BitmapRecognition(name, folder, is_sq, out parsed, out cells,
                                                      out coords, scale);
                    break;
                }
                catch (System.FormatException)
                {
                    parsed = null;
                    cells = null;
                    coords = null;
                    throw;
                }
            }
        }
    }
}
