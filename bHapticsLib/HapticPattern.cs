namespace bHapticsLib
{
    public class HapticPattern
    {
        public string Key { get; private set; }

        public static HapticPattern LoadFromFile(string key, string tactFilePath)
        {
            bHapticsManager.RegisterPatternFromFile(key, tactFilePath);
            return new HapticPattern { Key = key };
        }

        public static HapticPattern LoadFromJson(string key, string tactFileStr)
        {
            bHapticsManager.RegisterPatternFromJson(key, tactFileStr);
            return new HapticPattern { Key = key };
        }

        public static HapticPattern LoadMirroredFromFile(string key, string tactFilePath)
        {
            bHapticsManager.RegisterPatternMirroredFromFile(key, tactFilePath);
            return new HapticPattern { Key = key };
        }

        public static HapticPattern LoadMirroredFromJson(string key, string tactFileStr)
        {
            bHapticsManager.RegisterPatternMirroredFromJson(key, tactFileStr);
            return new HapticPattern { Key = key };
        }

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
