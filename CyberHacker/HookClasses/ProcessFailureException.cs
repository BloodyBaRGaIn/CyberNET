using System;

namespace CyberHacker.HookClasses
{
    internal sealed class ProcessFailureException : Exception
    {
        internal ProcessFailureException(string message) : base(message) { }
    }
}
