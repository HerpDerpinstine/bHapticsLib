﻿using System;

namespace bHapticsLib
{
    public class bHapticsException : Exception
    {
        public bHapticsException() { }
        public bHapticsException(string message) : base(message) { }
        public bHapticsException(string message, Exception exception) : base(message, exception) { }
    }
}