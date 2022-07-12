using bHapticsLib.SimpleJSON;

namespace bHapticsLib
{
    public class DotPoint : JSONObject
    {
        public DotPoint() { }
        public DotPoint(int index, int intensity = 50)
        {
            this.index = index;
            this.intensity = intensity;
        }

        public int index
        {
            get => this[nameof(index)].AsInt;
            set => this[nameof(index)] = value.Clamp(0, bHapticsManager.MaxMotorsPerPositionType);
        }

        public int intensity
        {
            get => this[nameof(intensity)].AsInt;
            set => this[nameof(intensity)] = value.Clamp(0, bHapticsManager.MaxIntensity);
        }
    }
}