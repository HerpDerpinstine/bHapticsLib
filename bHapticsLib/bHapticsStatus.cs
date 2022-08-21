namespace bHapticsLib
{
    /// <summary>Enum for Connection Status</summary>
#pragma warning disable IDE1006 // Naming Styles
    public enum bHapticsStatus : int
#pragma warning restore IDE1006 // Naming Styles
    {
        /// <summary>Disconnected from the bHaptics Player</summary>
        Disconnected = 0,

        /// <summary>Attempting to Connect to the bHaptics Player</summary>
        Connecting = 1,

        /// <summary>Connected to the bHaptics Player</summary>
        Connected = 2,
    }
}
