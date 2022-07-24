namespace bHapticsLib
{
    public class HapticPattern
    {
        public string Key { get; private set; }

        private HapticPattern() { }

        public static HapticPattern LoadFromFile(string filepath)
            => LoadFromFile(filepath, null);
        public static HapticPattern LoadFromFile(string filepath, string key)
        {
            KeyCheck(ref key);
            bHapticsManager.RegisterFeedbackFromFile(key, filepath);
            return new HapticPattern { Key = key };
        }

        public static HapticPattern LoadFromJson(string filestr)
            => LoadFromJson(filestr, null);
        public static HapticPattern LoadFromJson(string filestr, string key)
        {
            KeyCheck(ref key);
            bHapticsManager.RegisterFeedbackFromJson(key, filestr);
            return new HapticPattern { Key = key };
        }

        private static void KeyCheck(ref string key)
        {
            if (string.IsNullOrEmpty(key))
                key = Extensions.RandomString(12);

            int offset = 1;
            string originalKey = key;
            while (bHapticsManager.IsFeedbackRegistered(key))
            {
                offset++;
                key = $"{originalKey}_{offset}";
            }
        }

        public bool IsRegistered()
            => bHapticsManager.IsFeedbackRegistered(Key);
        public bool IsPlaying()
            => bHapticsManager.IsPlaying(Key);
        public void Play()
            => bHapticsManager.SubmitRegistered(Key);
        public void Stop()
            => bHapticsManager.StopPlaying(Key);
    }
}
