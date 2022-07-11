﻿using bHapticsLib.Internal.Connection.Models;
using bHapticsLib.SimpleJSON;
using System;
using System.Collections.Generic;

namespace bHapticsLib
{
    public static class Extensions
    {
        public static string ToOscAddress(this PositionType value)
        {
            switch (value)
            {
                // Head
                case PositionType.Head:
                    return "/head";

                // Vest
                case PositionType.Vest:
                    return "/vest";
                case PositionType.VestFront:
                    return "/vest/front";
                case PositionType.VestBack:
                    return "/vest/back";

                // Arms
                case PositionType.ForearmL:
                    return "/arm/left";
                case PositionType.ForearmR:
                    return "/arm/right";

                // Hands
                case PositionType.HandL:
                    return "/hand/left";
                case PositionType.HandR:
                    return "/hand/right";

                // Gloves
                case PositionType.GloveL:
                    return "/glove/left";
                case PositionType.GloveR:
                    return "/glove/right";

                // Feet
                case PositionType.FootL:
                    return "/foot/left";
                case PositionType.FootR:
                    return "/foot/right";

                // Custom
                case PositionType.Custom1:
                    return "/custom1";
                case PositionType.Custom2:
                    return "/custom2";
                case PositionType.Custom3:
                    return "/custom3";
                case PositionType.Custom4:
                    return "/custom4";

                // Unknown
                default:
                    return null;
            }
        }

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

        internal static void AddRange<T, Z>(this T arr, List<Z> value) where T : JSONNode where Z : JSONNode
        {
            if ((value == null) || arr.IsNull)
                return;
            int count = value.Count;
            if (count <= 0)
                return;
            for (int i = 0; i < count; i++)
            {
                Z node = value[i];
                if ((node == null) || node.IsNull)
                    continue;
                arr.Add(value[i]);
            }
        }

        internal static void AddRange<T, Z>(this T arr, Z[] value) where T : JSONNode where Z : JSONNode
        {
            if ((value == null) || arr.IsNull)
                return;
            int count = value.Length;
            if (count <= 0)
                return;
            for (int i = 0; i < count; i++)
            {
                Z node = value[i];
                if ((node == null) || node.IsNull)
                    continue;
                arr.Add(value[i]);
            }
        }

        internal static bool ContainsValue<T, Z>(this T arr, Z value) where T : JSONNode where Z : JSONNode
        {
            if (arr.IsNull || (value == null) || value.IsNull)
                return false;
            int count = arr.Count;
            if (count <= 0)
                return false;
            for (int i = 0; i < count; i++)
            {
                JSONNode node = arr[i];
                if ((node == null) || node.IsNull)
                    continue;
                if (value.IsObject && node.IsObject && (node.AsObject == value))
                    return true;
                if (value.IsArray && node.IsArray && (node.AsArray == value))
                    return true;
            }
            return false;
        }

        internal static bool ContainsValue<T>(this T arr, bool value) where T : JSONNode
        {
            if (arr.IsNull)
                return false;
            int count = arr.Count;
            if (count <= 0)
                return false;
            for (int i = 0; i < count; i++)
            {
                JSONNode node = arr[i];
                if ((node == null) || node.IsNull)
                    continue;
                if (node.IsBoolean && (node.AsBool == value))
                    return true;
            }
            return false;
        }

        internal static bool ContainsValue<T>(this T arr, string value) where T : JSONNode
        {
            if (arr.IsNull || string.IsNullOrEmpty(value))
                return false;
            int count = arr.Count;
            if (count <= 0)
                return false;
            for (int i = 0; i < count; i++)
            {
                JSONNode node = arr[i];
                if ((node == null) || node.IsNull)
                    continue;
                if (node.IsString && !string.IsNullOrEmpty(node.Value) && node.Value.Equals(value))
                    return true;
            }
            return false;
        }

        internal static bool ContainsValue<T>(this T arr, PositionType value) where T : JSONNode
            => ContainsValue(arr, value.ToString());
    }
}