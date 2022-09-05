using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib
{
    /// <summary>Scaling Option for Haptic Patterns</summary>
    public class ScaleOption
    {
        internal JSONObject node = new JSONObject();

        /// <summary>Scaling Option for Haptic Patterns</summary>
        /// <param name="intensity">Scale of Intensity, default is 1x</param>
        /// <param name="duration">Scale of Duration of Playback, default is 1x</param>
        public ScaleOption(float intensity = 1f, float duration = 1f)
        {
            Intensity = intensity;
            Duration = duration;
        }

        /// <value>Scale of Intensity, default is 1x</value>
        public float Intensity
        {
            get => node["intensity"].AsFloat;
            set => node["intensity"] = value;
        }

        /// <value>Scale of Duration of Playback, default is 1x</value>
        public float Duration
        {
            get => node["duration"].AsFloat;
            set => node["duration"] = value;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            => $"{nameof(ScaleOption)} ( {nameof(Intensity)}: {Intensity}, {nameof(Duration)}: {Duration} )";
    }
}