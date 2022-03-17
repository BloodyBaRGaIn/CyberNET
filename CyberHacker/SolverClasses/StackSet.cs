using System.Collections;
using System.Collections.Generic;

namespace CyberHacker.SolverClasses
{
    internal sealed class StackSet<T> : IEnumerable<T>
    {
        private const int InitialCapcity = 12;
        private readonly Stack<T> stack = new(InitialCapcity);
        private readonly HashSet<T> set = new(InitialCapcity);

        internal int Count => stack.Count;

        internal int Push(T v)
        {
            if (!set.Add(v))
            {
                throw new AlreadyInStackSetException($"'{v}' is already in the stack. This is a StackSet and does not allow duplicates. Use the Contains method to efficiently check for existence.");
            }
            stack.Push(v);
            return stack.Count;
        }

        internal T Pop()
        {
            T item = stack.Pop();
            _ = set.Remove(item);
            return item;
        }

        public bool Contains(T item) => set.Contains(item);

        public IEnumerator<T> GetEnumerator() => stack.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => stack.GetEnumerator();
    }
}
