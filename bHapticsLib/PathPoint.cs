using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib
{
    /// <summary>Haptic Point for Path Mode</summary>
    public class PathPoint
    {
        internal JSONObject node = new JSONObject();

        public PathPoint(float x = 0, float y = 0, int intensity = 50, int motorCount = 3)
        {
            X = x;
            Y = y;
            Intensity = intensity;
            MotorCount = motorCount;
        }

        public float X
        {
            get => node["x"].AsFloat;
            set => node["x"] = value;
        }

        public float Y
        {
            get => node["y"].AsFloat;
            set => node["y"] = value;
        }

        public int Intensity
        {
            get => node["intensity"].AsInt;
            set => node["intensity"] = value.Clamp(0, bHapticsManager.MaxIntensityInInt);
        }

        public int MotorCount
        {
            get => node["motorCount"].AsInt;
            set => node["motorCount"] = value.Clamp(0, bHapticsManager.MaxMotorsPerPathPoint);
        }

        public override string ToString()
            => $"{nameof(PathPoint)} ( {nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(MotorCount)}: {MotorCount}, {nameof(Intensity)}: {Intensity} )";
    }
}