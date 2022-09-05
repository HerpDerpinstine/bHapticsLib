using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib
{
    /// <summary>Rotational Option for Haptic Patterns</summary>
    public class RotationOption
    {
        internal JSONObject node = new JSONObject();

        /// <summary>Rotational Option for Haptic Patterns</summary>
        /// <param name="offsetAngleX">Rotation Angle X Axis</param>
        /// <param name="offsetY">Rotation Y Axis</param>
        public RotationOption(float offsetAngleX = 0, float offsetY = 0)
        {
            OffsetAngleX = offsetAngleX;
            OffsetY = offsetY;
        }

        /// <value>Rotation Angle X Axis</value>
        public float OffsetAngleX
        {
            get => node["offsetAngleX"].AsFloat;
            set => node["offsetAngleX"] = value;
        }

        /// <value>Rotation Y Axis</value>
        public float OffsetY
        {
            get => node["offsetY"].AsFloat;
            set => node["offsetY"] = value;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            => $"{nameof(RotationOption)} ( {nameof(OffsetAngleX)}: {OffsetAngleX}, {nameof(OffsetY)}: {OffsetY} )";
    }
}