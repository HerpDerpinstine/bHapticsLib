namespace bHapticsLib
{
    /// <summary>
    /// Enum for Connection Status
    /// </summary>
    public enum bHapticsStatus : int
    {
        /// <summary>
        /// Is Disconnected from the bHaptics Player
        /// </summary>
        Disconnected = 0,

        /// <summary>
        /// Is attempting to Connect to the bHaptics Player
        /// </summary>
        Connecting = 1,

        /// <summary>
        /// Is Connected to the bHaptics Player
        /// </summary>
        Connected = 2,
    }
}
