using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib
{
    /// <summary>Haptic Point for Dot Mode</summary>
    public class DotPoint
    {
        internal JSONObject node = new JSONObject();

        public DotPoint(int index = 0, int intensity = 50)
        {
            Index = index;
            Intensity = intensity;
        }

        /// <value>Index of Haptic Node</value>
        public int Index
        {
            get => node["index"].AsInt;
            set => node["index"] = value.Clamp(0, bHapticsManager.MaxMotorsPerDotPoint);
        }

        /// <value>Point Intensity</value>
        public int Intensity
        {
            get => node["intensity"].AsInt;
            set => node["intensity"] = value.Clamp(0, bHapticsManager.MaxIntensityInInt);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            => $"{nameof(DotPoint)} ( {nameof(Index)}: {Index}, {nameof(Intensity)}: {Intensity} )";
    }
}