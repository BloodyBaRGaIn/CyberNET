namespace CyberHacker.ExtensionClasses
{
    internal static class IntExtensions
    {
        internal static string ToHexString(this byte value) => value.ToString("X").PadLeft(2, '0');
    }
}
