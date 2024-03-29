﻿using System;
using System.Collections;
using System.Collections.Generic;
using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib
{
    /// <summary>Extra Extensions</summary>
    public static class Extensions
    {
        private static string OscAddressHeader = "/bhaptics";

        /// <summary>Converts PositionID to an Open Sound Control formatted address string.</summary>
        /// <returns>An Open Sound Control formatted address string.</returns>
        public static string ToOscAddress(this PositionID value)
            => (value) switch
            {
                // Head
                PositionID.Head => $"{OscAddressHeader}/head",

                // Vest
                PositionID.Vest => $"{OscAddressHeader}/vest",
                PositionID.VestFront => $"{OscAddressHeader}/vest/front",
                PositionID.VestBack => $"{OscAddressHeader}/vest/back",

                // Arms
                PositionID.ArmLeft => $"{OscAddressHeader}/arm/left",
                PositionID.ArmRight => $"{OscAddressHeader}/arm/right",

                // Hands
                PositionID.HandLeft => $"{OscAddressHeader}/hand/left",
                PositionID.HandRight => $"{OscAddressHeader}/hand/right",

                // Gloves
                PositionID.GloveLeft => $"{OscAddressHeader}/glove/left",
                PositionID.GloveRight => $"{OscAddressHeader}/glove/right",

                // Feet
                PositionID.FootLeft => $"{OscAddressHeader}/foot/left",
                PositionID.FootRight => $"{OscAddressHeader}/foot/right",

                // Unknown
                _ => null
            };

        internal static string ToPacketString(this PositionID value)
            => (value) switch
            {
                // Arms
                PositionID.ArmLeft => "ForearmL",
                PositionID.ArmRight => "ForearmR",

                // Hands
                PositionID.HandLeft => "HandL",
                PositionID.HandRight => "HandR",
                
                // Gloves
                PositionID.GloveLeft => "GloveL",
                PositionID.GloveRight => "GloveR",

                // Feet
                PositionID.FootLeft => "FootL",
                PositionID.FootRight => "FootR",

                // Default
                _ => value.ToString()
            };

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

        internal static bool ContainsValueMoreThan<T>(this T[] arr, T value) where T : IComparable<T>
        {
            int count = arr.Length;
            if (count <= 0)
                return false;
            for (int i = 0; i < count; i++)
                if (arr[i].CompareTo(value) > 0)
                    return true;
            return false;
        }

        internal static void ReverseAll<A>(this A arr) where A : IList, ICollection
            => arr.Reverse(0, arr.Count);
        internal static void Reverse<A>(this A arr, int index, int length) where A : IList, ICollection
        {
            int num = index;
            for (int i = index + length - 1; num < i; i--)
            {
                arr.Swap(i, num);
                num++;
            }
        }
        internal static void ReverseAll(this JSONNode node)
            => node.Reverse(0, node.Count);
        internal static void Reverse(this JSONNode node, int index, int length)
        {
            int num = index;
            for (int i = index + length - 1; num < i; i--)
            {
                JSONNode nodeA = node[i];
                JSONNode nodeB = node[num];
                node[i] = nodeB;
                node[num] = nodeA;
                num++;
            }
        }

        internal static void Swap<A>(this A dotPoints, int indexA, int indexB) where A : IList, ICollection
        {
            int count = dotPoints.Count;
            if (count <= 1)
                return;

            if ((indexA < 0) || (indexA > (count - 1)))
                return;
            if ((indexB < 0) || (indexB > (count - 1)))
                return;

            object d = dotPoints[indexA];
            object t = dotPoints[indexB];

            if (d.GetType() == typeof(DotPoint))
            {
                if (d != null)
                    (d as DotPoint).Index = indexB;

                if (t != null)
                    (t as DotPoint).Index = indexA;
            }

            dotPoints[indexB] = d;
            dotPoints[indexA] = t;
        }
    }
}