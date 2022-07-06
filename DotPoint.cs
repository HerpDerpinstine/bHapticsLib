using bHapticsLib.SimpleJSON;
using bHapticsLib.Internal;

namespace bHapticsLib
{
    public class DotPoint : JSONObject
    {
        public DotPoint(int index, int intensity)
        {
            if (index < 0)
                throw new bHapticsException("Invalid argument index : " + index);

            this.index = index;
            this.intensity = intensity.Clamp(0, bHapticsManager.MaxIntensity);
        }

        public int index
        {
            get => this[nameof(index)].AsInt;
            set => this[nameof(index)] = value;
        }

        public int intensity
        {
            get => this[nameof(intensity)].AsInt;
            set => this[nameof(intensity)] = value;
        }
    }
}