using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib
{
    public class DotPoint
    {
        internal JSONObject node = new JSONObject();

        public DotPoint(int index = 0, int intensity = 50)
        {
            Index = index;
            Intensity = intensity;
        }

        public int Index
        {
            get => node["index"].AsInt;
            set => node["index"] = value.Clamp(0, bHapticsManager.MaxMotorsPerDotPoint);
        }

        public int Intensity
        {
            get => node["intensity"].AsInt;
            set => node["intensity"] = value.Clamp(0, bHapticsManager.MaxIntensity);
        }

        public override string ToString()
        {
            return "DotPoint { Index: " + Index +
                   ", Intensity: " + Intensity + " }";
        }
    }
}