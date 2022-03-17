using System;

namespace CyberHacker.ExtensionClasses
{
    internal static class StringExtensions
    {
        internal static string FirstUpper(this string src)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                throw new ArgumentException(nameof(src));
            }
            string res = src[0].ToString().ToUpper();
            if (src.Length > 1)
            {
                res += src[1..];
            }
            return res;
        }

        internal static string TimeReport(this TimeSpan time, string pref, string suff = "recognition", string format = "0.000") =>
            $"{pref.FirstUpper()} {suff}: {time.TotalSeconds.ToString(format)} seconds{Constants.NL}";
    }
}
