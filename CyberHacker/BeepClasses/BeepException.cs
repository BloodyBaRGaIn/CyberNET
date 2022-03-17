using System;

namespace CyberHacker.BeepClasses
{
    sealed class BeepException : Exception
    {
        internal BeepException() : base() { }
        internal BeepException(string message) : base(message) { }
    }
}
