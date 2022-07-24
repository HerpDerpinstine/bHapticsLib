namespace bHapticsLib
{
    public class HapticPattern
    {
        public string Key { get; private set; }

        private HapticPattern() { }

        public static HapticPattern LoadFromFile(string key, string filepath)
        {
            bHapticsManager.RegisterFeedbackFromFile(key, filepath);
            return new HapticPattern { Key = key, };
        }

        public static HapticPattern LoadFromJson(string key, string filestr)
        {
            bHapticsManager.RegisterFeedbackFromJson(key, filestr);
            return new HapticPattern { Key = key, };
        }

        /*
        public static HapticPattern LoadMirroredFromFile(string key, string filepath)
        {
            bHapticsManager.RegisterFeedbackMirroredFromFile(key, filepath);
            return new HapticPattern { Key = key, };
        }

        public static HapticPattern LoadMirroredFromJson(string key, string filestr)
        {
            bHapticsManager.RegisterFeedbackMirroredFromJson(key, filestr);
            return new HapticPattern { Key = key, };
        }
        */

        public bool IsRegistered()
            => bHapticsManager.IsFeedbackRegistered(Key);
        public bool IsPlaying()
            => bHapticsManager.IsPlaying(Key);
        public void Stop()
            => bHapticsManager.StopPlaying(Key);

        public void Play()
            => bHapticsManager.SubmitRegistered(Key);
        public void Play(ScaleOption option) 
            => bHapticsManager.SubmitRegistered(Key, option);
        public void Play(RotationOption option) 
            => bHapticsManager.SubmitRegistered(Key, option);
        public void Play(ScaleOption scaleOption, RotationOption rotationOption) 
            => bHapticsManager.SubmitRegistered(Key, scaleOption, rotationOption);
    }
}
