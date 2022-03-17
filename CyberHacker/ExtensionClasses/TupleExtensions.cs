namespace CyberHacker.ExtensionClasses
{
    internal static class TupleExtensions
    {
        /// <summary>
        /// Gets a size from comlex-type parameter
        /// </summary>
        /// <param name="arg">A struct representing the size</param>
        /// <returns>A size based on parameter value</returns>
        internal static System.Drawing.Size StructSize(this (int, int) arg) => new(arg.Item1, arg.Item2);
        internal static (int, int) Swap(this (int a, int b) val, in bool sw) => sw ? (val.b, val.a) : val;
    }
}
