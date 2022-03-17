using CyberHacker.ExtensionClasses;
using System.Collections.Generic;
using System.Linq;

namespace CyberHacker.SolverClasses
{
    internal sealed class Attempt : TargetBase
    {
        private const int SQUISH_RESULT_CAPACITY = 25;
        internal new readonly int Weight;
        internal readonly byte[] Path;
        internal Attempt(IEnumerable<Target> targets)
        {
            Path = Squish(targets.Select(p => p.Values)).ToArray();
            Weight = targets.Select(p => p.Weight).Sum();
        }

        private static IEnumerable<byte> Squish(IEnumerable<byte[]> targets)
        {
            List<byte> result = new(SQUISH_RESULT_CAPACITY);
            result.AddRange(targets.First());
            foreach (byte[] target in targets.Skip(1))
            {
                int i_target = 0;
                for (int i_result = 0; i_result < result.Count && i_target < target.Length; i_result++)
                {
                    if (result[i_result] != target[i_target])
                    {
                        i_result -= i_target;
                        i_target = 0;
                    }
                    else
                    {
                        i_target++;
                    }
                }
                result.AddRange(target.Skip(i_target));
            }
            return result;
        }

        internal static IEnumerable<Attempt> GenerateAttempts(Target[] targets)
        {
            HashSet<Attempt> tried = new();
            return from IEnumerable<Target> path in targets.GetAllCombinationsAndPermutations()
                   let attempt = new Attempt(path)
                   where tried.Add(attempt) && attempt.Path != null && attempt.Path.Any()
                   select attempt;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
