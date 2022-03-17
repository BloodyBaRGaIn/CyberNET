using System;
using System.Linq;

namespace CyberHacker.SolverClasses
{
    abstract class TargetBase
    {
        protected virtual byte[] Values { get; }
        protected virtual int Weight { get; }
        public override bool Equals(object obj) => obj switch
        {
            Attempt attempt when this is Attempt attempt1 => attempt.Path.SequenceEqual(attempt1.Path) &&
            attempt.Weight == attempt1.Weight,
            Target target when this is Target target1 => target.Values.SequenceEqual(target1.Values) &&
            target.Weight == target1.Weight,
            _ => false
        };

        public override int GetHashCode() => this switch
        {
            Attempt attempt => HashCode.Combine(attempt.Path, attempt.Weight),
            Target target => HashCode.Combine(target.Values, target.Weight),
            _ => throw new NotImplementedException()
        };
    }
}
