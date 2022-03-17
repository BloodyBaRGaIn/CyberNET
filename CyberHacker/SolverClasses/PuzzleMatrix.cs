using CyberHacker.ExtensionClasses;
using System.Collections.Generic;
using System.Linq;

namespace CyberHacker.SolverClasses
{
    internal sealed class PuzzleMatrix
    {
        private readonly byte[,] input;
        private readonly int N;
        private int extraBuffer;
        private int bufferSize;

        internal PuzzleMatrix(in byte[,] input)
        {
            this.input = input;
            N = input.GetLength(0);
        }

        internal bool Check(in byte[] chain,
                            in int bufferSize,
                            out StackSet<ComplexIndex> path)
        {
            this.bufferSize = bufferSize;
            extraBuffer = bufferSize - chain.Length;

            path = new();
            return Check(chain, path);
        }

        private bool Check(byte[] chain, StackSet<ComplexIndex> path, int idx = 0, int i_chain = 0, bool is_row = true)
        {
            foreach ((int spotIndex, int i_chain_next, _) in from (int Index, int Value) spot in IndexCollection(idx, is_row)
                                                             let match = spot.Value == chain[i_chain]
                                                             let coords = (spot.Index, idx).Swap(is_row)
                                                             where !path.Contains(coords)
                                                                 && (path.Count < extraBuffer
                                                                 && i_chain == 0 || match)
                                                             select (spot.Index, i_chain + (match ? 1 : 0), path.Push(coords)))
            {
                bool pathFitsInBuffer = path.Count <= bufferSize;
                if (i_chain_next == chain.Length && pathFitsInBuffer)
                {
                    return true;
                }
                else if (!pathFitsInBuffer)
                {
                    _ = path.Pop();
                    return false;
                }
                else if (Check(chain, path, spotIndex, i_chain_next, !is_row))
                {
                    return true;
                }
                else
                {
                    _ = path.Pop();
                }
            }
            return false;
        }

        internal string Print(in IEnumerable<ComplexIndex> path)
        {
            byte[,] steps = new byte[N, N];
            {
                byte step = 0;
                foreach ((int X, int Y) in path)
                {
                    steps[X, Y] = ++step;
                }
            }
            string result = "";
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    result += $"{input[i, j].ToHexString()}{(steps[i, j] > 0 ? $"{-steps[i, j]}" : "  ")} ";
                }
                result += Constants.NL + Constants.NL;
            }
            return result;
        }

        private IEnumerable<(int, int)> IndexCollection(int idx, bool is_row)
        {
            for (int i = 0; i < N; i++)
            {
                (int a, int b) = (i, idx).Swap(is_row);
                yield return (i, input[a, b]);
            }
        }
    }
}
