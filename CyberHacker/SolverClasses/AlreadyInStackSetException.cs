using System;

namespace CyberHacker.SolverClasses
{
    internal sealed class AlreadyInStackSetException : Exception
    {
        internal AlreadyInStackSetException(string message) : base(message) { }
    }
}
