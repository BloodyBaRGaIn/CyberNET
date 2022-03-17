namespace CyberHacker.SolverClasses
{
    internal sealed class Target : TargetBase
    {
        internal new readonly byte[] Values;
        internal new readonly int Weight;

        internal Target(byte[] Values = null, int Weight = 0)
        {
            this.Values = Values ?? new byte[0];
            this.Weight = Weight;
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
