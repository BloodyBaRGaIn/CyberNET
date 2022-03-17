using CyberHacker.ExtensionClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberHacker.SolverClasses
{
    /// <summary>
    /// Provides a solution finding method
    /// </summary>
    internal static class PathFinder
    {
        /// <summary>
        /// Finds the solution
        /// </summary>
        /// <param name="inputSplit">An array of input data as strings</param>
        /// <param name="bitmap">A <see cref="ParsedBitmap"/> object to set the solution path</param>
        /// <param name="completed">A collection of sequences data (is it completed or not and the weight of it)</param>
        /// <returns>An output log text</returns>
        internal static string Find(in string[] inputSplit, ref BitmapProcClasses.ParsedBitmap bitmap, out List<(bool, int)> completed)
        {
            completed = new();
            IEnumerable<ComplexIndex> ps = new ComplexIndex[1] { new() };
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            PuzzleMatrix matrix = new PuzzleMatrix(InputParser.ParseMatrix(inputSplit[0]));
            Target[] targets = InputParser.ParseTargets(inputSplit[1])
                                          .Where(item => item != null && item?.Values.Length > 0 && item?.Weight != 0)
                                          .ToArray();
            string result = "";
            PuzzleSolution solution = null;
            if (!int.TryParse(inputSplit[2].Trim(), out int bufferSize))
            {
                result += $"Invalid buffer size value{Constants.NL}";
            }
            else if (!targets.Any())
            {
                result += $"No targets found{Constants.NL}";
            }
            else
            {
                solution = PuzzleSolution.GetBestSolution(matrix, targets, bufferSize, out int init_cnt);
                stopwatch.Stop();
                #region Forming output string
                result += stopwatch.Elapsed.TimeReport("Found", $"{init_cnt} valid solution(s)");
                result += $"Best solution:{Constants.NL}";
                result += $"\tweight {solution.Attempt.Weight}{Constants.NL}";
                result += $"\tlength {solution.Path.Count}{Constants.NL}";
                result += $"\tbuffer {bufferSize}{Constants.NL}";
                #endregion
                #region Forming list of completed targets
                foreach (Target v in targets)
                {
                    completed.Add((solution.Attempt.Path.ContainsSequence(v.Values), v.Weight));
                }
                #endregion
            }
            result += Constants.NL;
            if (solution != null)
            {
                ps = solution.Path.Reverse();
            }
            result += matrix.Print(ps);
            bitmap.SetSolutionPath(ps);
            return result;
        }
    }
}
