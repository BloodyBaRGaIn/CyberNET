namespace CyberHacker.ComparableClasses
{
    internal interface IToleranceComparable
    {
        public abstract bool Compare(object a, int tolerance);
    }
}
