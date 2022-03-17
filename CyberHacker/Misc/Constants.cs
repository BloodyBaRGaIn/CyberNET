using System;
using System.Drawing;

namespace CyberHacker
{
    internal static class Constants
    {
        internal static readonly string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory.Trim('\\');
        internal static readonly string FriendlyName = AppDomain.CurrentDomain.FriendlyName;
        internal static readonly Font font = new(new FontFamily("Yu Gothic UI"), 11, FontStyle.Bold);
        internal const string NL = "\r\n";
    }
}
