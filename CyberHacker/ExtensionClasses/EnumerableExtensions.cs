using CyberHacker.ComparableClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberHacker.ExtensionClasses
{
    internal static class EnumerableExtensions
    {
        private static void SetIndexes(int[] indexes, int lastIndex, int count)
        {
            indexes[lastIndex]++;
            if (lastIndex > 0 && indexes[lastIndex] == count)
            {
                SetIndexes(indexes, lastIndex - 1, count - 1);
                indexes[lastIndex] = indexes[lastIndex - 1] + 1;
            }
        }

        private static bool AllPlacesChecked(int[] indexes, int places)
        {
            for (int i = indexes.Length - 1; i >= 0; i--)
            {
                if (indexes[i] != places)
                {
                    return false;
                }
                places--;
            }
            return true;
        }

        internal static IEnumerable<IEnumerable<T>> GetDifferentCombinations<T>(this IEnumerable<T> c, int count)
        {
            int listCount = c.Count();
            if (count > listCount)
            {
                throw new InvalidOperationException($"{nameof(count)} is greater than the collection elements.");
            }
            int[] indexes = Enumerable.Range(0, count).ToArray();
            do
            {
                yield return indexes.Select(i => c.ElementAt(i));
                SetIndexes(indexes, indexes.Length - 1, listCount);
            }
            while (!AllPlacesChecked(indexes, listCount));
        }

        internal static IEnumerable<IEnumerable<T>> GetAllCombinationsAndPermutations<T>(this IEnumerable<T> c)
        {
            for (int i = c.Count(); i > 0; i--)
            {
                foreach (IEnumerable<T> p in from IEnumerable<T> j in c.GetDifferentCombinations(i)
                                             from IEnumerable<T> p in j.Permute()
                                             select p)
                {
                    yield return p;
                }
            }
        }

        internal static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> set, IEnumerable<T> subset = null)
        {
            if (subset == null)
            {
                subset = Array.Empty<T>();
            }
            if (!set.Any())
            {
                yield return subset;
            }
            for (int i = 0; i < set.Count(); i++)
            {
                var newSubset = set.Take(i).Concat(set.Skip(i + 1));
                foreach (var permutation in Permute(newSubset, subset.Concat(set.Skip(i).Take(1))))
                {
                    yield return permutation;
                }
            }
        }

        internal static IEnumerable<T> Add<T>(this IEnumerable<T> collection, T value)
        {
            foreach (var item in collection)
            {
                yield return item;
            }
            yield return value;
        }

        internal static int DistinctCount<T>(this IEnumerable<T> ts) => ts.Distinct().Count();

        internal static bool ContainsSequence<T>(this IEnumerable<T> source, IEnumerable<T> desired)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (desired is null)
            {
                throw new ArgumentNullException(nameof(desired));
            }
            int src_cnt = source.Count();
            int des_cnt = desired.Count();
            if (src_cnt == 0)
            {
                throw new ArgumentException("The source sequence contains no elements");
            }
            if (des_cnt == 0)
            {
                throw new ArgumentException("The desired sequence contains no elements");
            }
            if (src_cnt < des_cnt)
            {
                throw new ArgumentException("The length of the source sequence is less than the length of the desired one");
            }
            if (des_cnt == 1)
            {
                return source.Contains(desired.FirstOrDefault());
            }
            for (int i = 0; i <= src_cnt - des_cnt; i++)
            {
                if (source.Skip(i).Take(des_cnt).SequenceEqual(desired))
                {
                    return true;
                }
            }
            return false;
        }

#nullable enable annotations
        internal static T? RandomElement<T>(this IEnumerable<T> set) =>
            (set != null && set.Any()) ? set.ElementAtOrDefault(new Random().Next(0, set.Count())) : default;
#nullable restore annotations

        internal static object FirstOrNull<T>(this IEnumerable<T> set)
        {
            try
            {
                return set.First();
            }
            catch
            {
                return null;
            }
        }

        internal static IEnumerable<T> CuttedList<T>(this IEnumerable<T> coords, int tolerance) where T : IToleranceComparable
        {
            List<T> result_list = coords.ToList();
            for (int i = 0; i < result_list.Count - 1; i++)
            {
                for (int j = i + 1; j < result_list.Count; j++)
                {
                    if (result_list[i].Compare(result_list[j], tolerance))
                    {
                        result_list.RemoveAt(j);
                        i--;
                        break;
                    }
                }
            }
            return result_list.Distinct();
        }
    }
}
