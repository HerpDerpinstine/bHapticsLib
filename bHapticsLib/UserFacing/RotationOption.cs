using bHapticsLib.SimpleJSON;

namespace bHapticsLib
{
    public class RotationOption : JSONObject
    {
        public RotationOption(float offsetAngleX, float offsetY)
        {
            this.offsetAngleX = offsetAngleX;
            this.offsetY = offsetY;
        }

        public float offsetAngleX
        {
            get => this[nameof(offsetAngleX)].AsFloat;
            set => this[nameof(offsetAngleX)] = value;
        }

        public float offsetY
        {
            get => this[nameof(offsetY)].AsFloat;
            set => this[nameof(offsetY)] = value;
        }
    }
}