using bHapticsLib.SimpleJSON;
using System;

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

        internal static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;
            return value;
        }

        internal static bool ContainsValue(this JSONNode arr, bool value)
        {
            JSONNode.ValueEnumerator enumerator = arr.Values;
            while (enumerator.MoveNext())
            {
                JSONNode currentNode = enumerator.Current;
                if (currentNode.IsNull)
                    continue;
                if (currentNode.IsBoolean && (currentNode.AsBool == value))
                    return true;
            }
            return false;
        }

        internal static bool ContainsValue(this JSONNode arr, string value)
        {
            JSONNode.ValueEnumerator enumerator = arr.Values;
            while (enumerator.MoveNext())
            {
                JSONNode currentNode = enumerator.Current;
                if ((currentNode == null) || currentNode.IsNull)
                    continue;
                if (currentNode.IsString && (currentNode.Value != null) && currentNode.Value.Equals(value))
                    return true;
            }
            return false;
        }

        internal static bool ContainsValue(this JSONNode arr, PositionType value)
        {
            JSONNode.ValueEnumerator enumerator = arr.Values;
            while (enumerator.MoveNext())
            {
                JSONNode currentNode = enumerator.Current;
                if ((currentNode == null) || currentNode.IsNull)
                    continue;
                if (currentNode.IsNumber && ((PositionType)currentNode.AsInt == value))
                    return true;
            }
            return false;
        }
    }
}
