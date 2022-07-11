using bHapticsLib.SimpleJSON;

namespace bHapticsLib
{
    public class PathPoint : JSONObject
    {
        public PathPoint() { }
        public PathPoint(float x, float y, int intensity = 50, int motorCount = 3)
        {
            this.x = x;
            this.y = y;
            this.intensity = intensity.Clamp(0, bHapticsManager.MaxIntensity);
            this.motorCount = motorCount.Clamp(0, bHapticsManager.MaxMotorsPerPathPoint);
        }

        public float x
        {
            get => this[nameof(x)].AsFloat;
            set => this[nameof(x)] = value;
        }

        public float y
        {
            get => this[nameof(y)].AsFloat;
            set => this[nameof(y)] = value;
        }

        public int intensity
        {
            get => this[nameof(intensity)].AsInt;
            set => this[nameof(intensity)] = value.Clamp(0, bHapticsManager.MaxIntensity);
        }

        public int motorCount
        {
            get => this[nameof(motorCount)].AsInt;
            set => this[nameof(motorCount)] = value.Clamp(0, bHapticsManager.MaxMotorsPerPathPoint);
        }
    }
}