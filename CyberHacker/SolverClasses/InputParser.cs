using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CyberHacker.SolverClasses
{
    internal static class InputParser
    {
        private static readonly string[] Splitters = new string[3] { Constants.NL, "\n", " " };
        private static readonly StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries;
        internal static byte[,] ParseMatrix(in string input)
        {
            string[] parts = input.Split(Splitters, options);
            int n = (int)Math.Sqrt(parts.Length);
            if (n * n != parts.Length)
            {
                throw new FormatException("The puzzle input was not a square matrix.");
            }
            byte[,] m = new byte[n, n];
            for (int i = 0; i < parts.Length; i++)
            {
                if (!byte.TryParse(parts[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out m[i / n, i % n]))
                {
                    throw new HexParsingException($"Cannot parse {parts[i]} as hex number.");
                }
            }
            return m;
        }

        internal static IEnumerable<Target> ParseTargets(in string input)
        {
            IList<Target> targets = new List<Target>();
            string[] array = input.Split(Splitters[0..1], options);
            for (int i = 0; i < array.Length; i++)
            {
                string line = array[i];
                try
                {
                    byte[] values = line.Split(Splitters[2], options)
                                        .Select(num => byte.Parse(num.Trim(), NumberStyles.HexNumber)).ToArray();
                    targets.Add(new(values, (i * (i + 1)) + 1));
                }
                catch (FormatException)
                {
                    throw new HexParsingException($"An item in {line} could not be parsed as hex.");
                }
            }
            return targets;
        }
    }
}
