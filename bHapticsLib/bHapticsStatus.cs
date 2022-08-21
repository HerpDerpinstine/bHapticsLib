namespace bHapticsLib
{
    /// <summary>Enum for Connection Status</summary>
    public enum bHapticsStatus : int
    {
        /// <summary>Disconnected from the bHaptics Player</summary>
        Disconnected = 0,

        /// <summary>Attempting to Connect to the bHaptics Player</summary>
        Connecting = 1,

        /// <summary>Connected to the bHaptics Player</summary>
        Connected = 2,
    }
}
