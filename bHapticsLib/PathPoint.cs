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

        /// <value>X Axis of Position</value>
        public float X
        {
            get => node["x"].AsFloat;
            set => node["x"] = value;
        }

        /// <value>Y Axis of Position</value>
        public float Y
        {
            get => node["y"].AsFloat;
            set => node["y"] = value;
        }

        /// <value>Point Intensity</value>
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            => $"{nameof(PathPoint)} ( {nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(MotorCount)}: {MotorCount}, {nameof(Intensity)}: {Intensity} )";
    }
}