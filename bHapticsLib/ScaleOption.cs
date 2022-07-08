using bHapticsLib.SimpleJSON;

namespace bHapticsLib
{
    public class ScaleOption : JSONObject
    {
        public ScaleOption(float intensity = 1f, float duration = 1f)
        {
            this.intensity = intensity;
            this.duration = duration;
        }

        public float intensity
        {
            get => this[nameof(intensity)].AsFloat;
            set => this[nameof(intensity)] = value;
        }

        public float duration
        {
            get => this[nameof(duration)].AsFloat;
            set => this[nameof(duration)] = value;
        }
    }
}