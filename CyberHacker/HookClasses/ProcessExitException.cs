using System;

namespace CyberHacker.HookClasses
{
    internal sealed class ProcessExitException : Exception
    {
        internal ProcessExitException() : base(string.Empty) { }
    }
}
