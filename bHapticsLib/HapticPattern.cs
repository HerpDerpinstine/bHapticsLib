namespace bHapticsLib
{
    public class HapticPattern
    {
        private HapticPattern() { }
        public string Key { get; private set; }

        public static HapticPattern LoadFromFile(string key, string filepath)
        {
            bHapticsManager.RegisterPatternFromFile(key, filepath);
            return new HapticPattern { Key = key };
        }

        public static HapticPattern LoadFromJson(string key, string filestr)
        {
            bHapticsManager.RegisterPatternFromJson(key, filestr);
            return new HapticPattern { Key = key };
        }

        public static HapticPattern CloneFromRegisteredPattern(string key)
        {
            if (!bHapticsManager.IsPatternRegistered(key))
                return null; // To-Do: Exception Here
            return new HapticPattern { Key = key };
        }

        /*
        public static HapticPattern LoadMirroredFromFile(string key, string filepath)
        {
            bHapticsManager.RegisterPatternMirroredFromFile(key, filepath);
            return new HapticPattern { Key = key, };
        }

        public static HapticPattern LoadMirroredFromJson(string key, string filestr)
        {
            bHapticsManager.RegisterPatternMirroredFromJson(key, filestr);
            return new HapticPattern { Key = key, };
        }

        public static HapticPattern CloneMirroredFromRegisteredPattern(string key)
        {

        }
        */

        public bool IsRegistered()
            => bHapticsManager.IsPatternRegistered(Key);
        public bool IsPlaying()
            => bHapticsManager.IsPlaying(Key);
        public void Stop()
            => bHapticsManager.StopPlaying(Key);

        public void Play()
            => bHapticsManager.PlayRegistered(Key);
        public void Play(ScaleOption option) 
            => bHapticsManager.PlayRegistered(Key, option);
        public void Play(RotationOption option) 
            => bHapticsManager.PlayRegistered(Key, option);
        public void Play(ScaleOption scaleOption, RotationOption rotationOption) 
            => bHapticsManager.PlayRegistered(Key, scaleOption, rotationOption);
    }
}
