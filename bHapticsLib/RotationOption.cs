using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib
{
    public class RotationOption
    {
        internal JSONObject node = new JSONObject();

        public RotationOption(float offsetAngleX = 0, float offsetY = 0)
        {
            OffsetAngleX = offsetAngleX;
            OffsetY = offsetY;
        }

        public float OffsetAngleX
        {
            get => node["offsetAngleX"].AsFloat;
            set => node["offsetAngleX"] = value;
        }

        public float OffsetY
        {
            get => node["offsetY"].AsFloat;
            set => node["offsetY"] = value;
        }

        public override string ToString()
        {
            return "RotationOption { OffsetAngleX: " + OffsetAngleX +
                   ", OffsetY: " + OffsetY + " }";
        }
    }
}