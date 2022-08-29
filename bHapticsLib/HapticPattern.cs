namespace bHapticsLib
{
    /// <summary>Haptic Pattern Utility</summary>
    public class HapticPattern
    {
        /// <value>Pattern key id</value>
        public string Key { get; private set; }

        /// <summary>Loads a pattern from raw json into HapticPattern</summary>
        /// <param name="key">The key id of the pattern</param>
        /// <param name="tactFileJson">The raw json of the pattern</param>
        public static HapticPattern LoadFromJson(string key, string tactFileJson)
        {
            bHapticsManager.RegisterPatternFromJson(key, tactFileJson);
            return new HapticPattern { Key = key };
        }

        /// <summary>Loads a pattern from file path into HapticPattern</summary>
        /// <param name="key">The key id of the pattern</param>
        /// <param name="tactFilePath">The file path of the pattern</param>
        public static HapticPattern LoadFromFile(string key, string tactFilePath)
        {
            bHapticsManager.RegisterPatternFromFile(key, tactFilePath);
            return new HapticPattern { Key = key };
        }

        /// <summary>Loads a pattern swapped from raw json into HapticPattern</summary>
        /// <param name="key">The key id of the pattern</param>
        /// <param name="tactFileJson">The raw json of the pattern</param>
        public static HapticPattern LoadSwappedFromJson(string key, string tactFileJson)
        {
            bHapticsManager.RegisterPatternSwappedFromJson(key, tactFileJson);
            return new HapticPattern { Key = key };
        }

        /// <summary>Loads a pattern swapped from file path into HapticPattern</summary>
        /// <param name="key">The key id of the pattern</param>
        /// <param name="tactFilePath">The file path of the pattern</param>
        public static HapticPattern LoadSwappedFromFile(string key, string tactFilePath)
        {
            bHapticsManager.RegisterPatternSwappedFromFile(key, tactFilePath);
            return new HapticPattern { Key = key };
        }

        /// <summary>Checks if the HapticPattern is Registered</summary>
        public bool IsRegistered()
            => bHapticsManager.IsPatternRegistered(Key);

        /// <summary>Checks if the HapticPattern is Playing</summary>
        public bool IsPlaying()
            => bHapticsManager.IsPlaying(Key);

        /// <summary>Stops the HapticPattern</summary>
        public void Stop()
            => bHapticsManager.StopPlaying(Key);

        /// <summary>Plays the HapticPattern</summary>
        public void Play()
            => bHapticsManager.PlayRegistered(Key);

        /// <summary>Plays the HapticPattern</summary>
        /// <param name="option">Custom Playback Scale Option</param>
        public void Play(ScaleOption option) 
            => bHapticsManager.PlayRegistered(Key, option);

        /// <summary>Plays the HapticPattern</summary>
        /// <param name="option">Custom Playback Rotation Option</param>
        public void Play(RotationOption option) 
            => bHapticsManager.PlayRegistered(Key, option);

        /// <summary>Plays the HapticPattern</summary>
        /// <param name="scaleOption">Custom Playback Scale Option</param>
        /// <param name="rotationOption">Custom Playback Rotation Option</param>
        public void Play(ScaleOption scaleOption, RotationOption rotationOption) 
            => bHapticsManager.PlayRegistered(Key, scaleOption, rotationOption);
    }
}
