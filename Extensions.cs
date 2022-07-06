﻿using System;

namespace bHapticsLib
{
    public static class Extensions
    {
        internal static T Clamp<T>(T value, T min, T max) where T : IComparable<T> { if (value.CompareTo(min) < 0) return min; if (value.CompareTo(max) > 0) return max; return value; }
        internal static Int16 Clamp(this Int16 value, Int16 min, Int16 max)
            => Clamp<Int16>(value, min, max);
        internal static UInt16 Clamp(this UInt16 value, UInt16 min, UInt16 max)
            => Clamp<UInt16>(value, min, max);
        internal static Int32 Clamp(this Int32 value, Int32 min, Int32 max)
            => Clamp<Int32>(value, min, max);
        internal static UInt32 Clamp(this UInt32 value, UInt32 min, UInt32 max)
            => Clamp<UInt32>(value, min, max);
        internal static Double Clamp(this Double value, Double min, Double max)
            => Clamp<Double>(value, min, max);
        internal static Single Clamp(this Single value, Single min, Single max)
            => Clamp<Single>(value, min, max);
    }
}
