using System;

namespace bHapticsLib
{
    /// <summary>Exception Thrown from bHapticsLib</summary>
    public class bHapticsException : Exception
    {
        internal bHapticsException() { }
        internal bHapticsException(string message) : base(message) { }
        internal bHapticsException(string message, Exception exception) : base(message, exception) { }
    }
}
