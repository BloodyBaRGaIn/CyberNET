using CyberHacker.ExtensionClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberHacker.SolverClasses
{
    sealed class PuzzleSolution
    {
        internal readonly Attempt Attempt;
        internal readonly StackSet<ComplexIndex> Path;

        private PuzzleSolution(Attempt Attempt, StackSet<ComplexIndex> Path)
        {
            this.Attempt = Attempt;
            this.Path = Path;
        }

        /// <summary>
        /// Gets all possible solutions
        /// </summary>
        /// <param name="matrix">A square matrix of elements to select</param>
        /// <param name="targets">A collection of targets to complete</param>
        /// <param name="bufferSize">A maximum length of the solution</param>
        /// <returns>A collection of the solutions</returns>
        private static IEnumerable<PuzzleSolution> GetSolutions(PuzzleMatrix matrix, Target[] targets, int bufferSize)
        {
            foreach (Attempt attempt in Attempt.GenerateAttempts(targets))
            {
                for (int temp_size = targets.Max(item => item.Values.Length); temp_size <= bufferSize; temp_size++)
                {
                    if (matrix.Check(attempt.Path, temp_size, out var path))
                    {
                        yield return new PuzzleSolution(attempt, path);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the best solution
        /// </summary>
        /// <param name="matrix">A square matrix of elements to select</param>
        /// <param name="targets">A collection of targets to complete</param>
        /// <param name="bufferSize">A maximum length of the solution</param>
        /// <param name="init_cnt">A number of the solutions. If it's greater than 1, the method will return a randomly selected one</param>
        /// <returns>The best solution represented as <see cref="PuzzleSolution"/> object instance</returns>
        internal static PuzzleSolution GetBestSolution(PuzzleMatrix matrix, Target[] targets, int bufferSize, out int init_cnt)
        {
            //Input check
            if (matrix is null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }
            if (targets is null)
            {
                throw new ArgumentNullException(nameof(targets));
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentException(nameof(bufferSize));
            }
            List<PuzzleSolution> solutions = GetSolutions(matrix, targets, bufferSize).ToList();
            if (solutions.Count == 0)
            {
                throw new InvalidOperationException("There is no valid solution to this puzzle");
            }
            //Distinct
            for (int i = 0; i < solutions.Count - 1; i++)
            {
                for (int j = i + 1; j < solutions.Count; j++)
                {
                    if (solutions[i].Equals(solutions[j]))
                    {
                        solutions.RemoveAt(j--);
                    }
                }
            }
            //Maximum weight
            if (solutions.Count > 1)
            {
                int max_weight = solutions.Max(item => item.Attempt.Weight);
                solutions.RemoveAll(item => item.Attempt.Weight != max_weight);
            }
            //Minimum buffer cells occupied
            if (solutions.Count > 1)
            {
                int min_len = solutions.Min(item => item.Path.Count);
                solutions.RemoveAll(item => item.Path.Count != min_len);
            }
            //Minimum distance between elements
            if (solutions.Count > 1)
            {
                int min_dist = solutions.Min(item => TotalDistance(item.Path));
                solutions.RemoveAll(item => TotalDistance(item.Path) != min_dist);
            }
            init_cnt = solutions.Count;
            return solutions.RandomElement();
        }

        /// <summary>
        /// Gets a total distance between pairs of the <see cref="ComplexIndex"/> elements
        /// </summary>
        /// <param name="ps">A given collection</param>
        /// <returns></returns>
        private static int TotalDistance(IEnumerable<ComplexIndex> ps)
        {
            if (ps == null || !ps.Any())
            {
                throw new ArgumentNullException(nameof(ps));
            }
            int distance = 0, cnt = ps.Count() - 1;
            for (int i = 0; i < cnt; i++)
            {
                distance += ComplexIndex.Distance(ps.ElementAt(i), ps.ElementAt(i + 1));
            }
            return distance;
        }

        public override bool Equals(object obj)
        {
            return obj is PuzzleSolution solution && Attempt.Equals(solution.Attempt);
        }

        public static implicit operator (Attempt att, StackSet<ComplexIndex> path)(PuzzleSolution value)
        {
            return (value.Attempt, value.Path);
        }

        public static implicit operator PuzzleSolution((Attempt att, StackSet<ComplexIndex> path) value)
        {
            return new PuzzleSolution(value.att, value.path);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Attempt, Path);
        }
    }
}
