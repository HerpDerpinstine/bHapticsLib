using System;
using System.Collections.Generic;

namespace bHapticsLib
{
    /// <summary>AIO bHaptics Management</summary>
#pragma warning disable IDE1006 // Naming Styles
    public static class bHapticsManager
#pragma warning restore IDE1006 // Naming Styles
    {
        #region Max Values
        /// <value>Max Intensity of Point in Integer Array</value>
        public const int MaxIntensityInInt = 500;
        /// <value>Max Intensity of Point in Byte Array</value>
        public const byte MaxIntensityInByte = 200;
        /// <value>Max Motors per DotPoint</value>
        public const int MaxMotorsPerDotPoint = 20;
        /// <value>Max Motors per PathPoint</value>
        public const int MaxMotorsPerPathPoint = 3;
        #endregion

        #region Connection
        private static bHapticsConnection Connection = new bHapticsConnection();

        /// <value>Current Status of Connection</value>
        public static bHapticsStatus Status { get => Connection.Status; }

        /// <summary>Connects to the bHaptics Player</summary>
        /// <param name="id">Application Identifier</param>
        /// <param name="name">Application Name</param>
        /// <param name="tryToReconnect">If you want the Connection to Automatically Retry after Failure</param>
        /// <param name="maxRetries">The amount of Retries after Failure before Disconnecting</param>
        /// <returns>true was Successful, otherwise false</returns>
        public static bool Connect(string id, string name, bool tryToReconnect = true, int maxRetries = 5)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (Status != bHapticsStatus.Disconnected)
                Disconnect();

            Connection.ID = id;
            Connection.Name = name;
            Connection.TryToReconnect = tryToReconnect;
            Connection.MaxRetries = maxRetries.Clamp(0, int.MaxValue);

            return Connection.BeginInit();
        }

        /// <summary>Disconnects from the bHaptics Player</summary>
        /// <returns>true was Successful, otherwise false</returns>
        public static bool Disconnect()
        {
            if (Status == bHapticsStatus.Disconnected)
                return true;

            StopPlayingAll();
            return Connection.EndInit();
        }
        #endregion

        #region Device
        /// <summary>Gets the total amount of devices currently connected to the bHaptics Player</summary>
        /// <returns>The total amount of devices currently connected to the bHaptics Player</returns>
        public static int GetConnectedDeviceCount() 
            => Connection.GetConnectedDeviceCount();

        /// <summary>Gets if any devices are currently connected to the bHaptics Player</summary>
        /// <returns>true if there is a device, otherwise false</returns>
        public static bool IsAnyDevicesConnected() 
            => GetConnectedDeviceCount() > 0;

        /// <summary>Gets if a specific device is currently connected to the bHaptics Player</summary>
        /// <returns>true if the device is connected, otherwise false</returns>
        public static bool IsDeviceConnected(PositionID type) 
            => Connection.IsDeviceConnected(type);

        /// <summary>Gets the current status a specific device.</summary>
        /// <returns>An Integer Array containing the current intensity value for each motor of the device</returns>
        public static int[] GetDeviceStatus(PositionID type)
            => Connection.GetDeviceStatus(type);

        /// <summary>Gets if any motor of a specific device is currently at an intensity value of more than 0</summary>
        /// <returns>true if there is a motor active, otherwise false</returns>
        public static bool IsAnyMotorActive(PositionID type)
            => GetDeviceStatus(type)?.ContainsValueMoreThan(0) ?? false;
#endregion

        #region IsPlaying
        /// <summary>Gets if a specified pattern is currently playing</summary>
        /// <param name="key">The key id of the pattern</param>
        /// <returns>true if the specified pattern is playing, otherwise false</returns>
        public static bool IsPlaying(string key) 
            => Connection.IsPlaying(key);

        /// <summary>Gets if any pattern is currently playing</summary>
        /// <returns>true if any pattern is playing, otherwise false</returns>
        public static bool IsPlayingAny()
            => Connection.IsPlayingAny();
        #endregion

        #region StopPlaying
        /// <summary>Stops the specified pattern</summary>
        /// <param name="key">The key id of the pattern</param>
        public static void StopPlaying(string key) 
            => Connection.StopPlaying(key);

        /// <summary>Stops the currently playing patterns</summary>
        public static void StopPlayingAll() 
            => Connection.StopPlayingAll();
        #endregion

        #region RegisterPattern
        public static bool IsPatternRegistered(string key) 
            => Connection.IsPatternRegistered(key);
        public static void RegisterPatternFromJson(string key, string tactFileStr)
            => Connection.RegisterPatternFromJson(key, tactFileStr);
        public static void RegisterPatternFromFile(string key, string tactFilePath)
            => Connection.RegisterPatternFromFile(key, tactFilePath);
        public static void RegisterPatternSwappedFromJson(string key, string tactFileStr)
            => Connection.RegisterPatternSwappedFromJson(key, tactFileStr);
        public static void RegisterPatternSwappedFromFile(string key, string tactFilePath)
            => Connection.RegisterPatternSwappedFromFile(key, tactFilePath);
        #endregion

        #region PlayDot
        public static void Play(string key, int durationMillis, PositionID position, int[] dotPoints) 
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null);
        public static void Play(string key, int durationMillis, PositionID position, List<int> dotPoints)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null);
        public static void Play(string key, int durationMillis, PositionID position, byte[] dotPoints)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null);
        public static void Play(string key, int durationMillis, PositionID position, List<byte> dotPoints)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null);
        public static void Play(string key, int durationMillis, PositionID position, DotPoint[] dotPoints)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null);
        public static void Play(string key, int durationMillis, PositionID position, List<DotPoint> dotPoints)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null);
        #endregion

        #region PlayPath
        public static void Play<A>(string key, int durationMillis, PositionID position, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, (DotPoint[])null, pathPoints);
        #endregion

        #region PlayDotAndPath
        public static void Play<A>(string key, int durationMillis, PositionID position, int[] dotPoints, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints);
        public static void Play<A>(string key, int durationMillis, PositionID position, List<int> dotPoints, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints);
        public static void Play<A>(string key, int durationMillis, PositionID position, byte[] dotPoints, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints);
        public static void Play<A>(string key, int durationMillis, PositionID position, List<byte> dotPoints, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints);
        public static void Play<A>(string key, int durationMillis, PositionID position, DotPoint[] dotPoints, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints);
        public static void Play<A>(string key, int durationMillis, PositionID position, List<DotPoint> dotPoints, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints);
        #endregion

        #region PlayMirroredDot
        public static void PlayMirrored(string key, int durationMillis, PositionID position, int[] dotPoints, MirrorDirection mirrorDirection)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionID position, List<int> dotPoints, MirrorDirection mirrorDirection)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionID position, byte[] dotPoints, MirrorDirection mirrorDirection)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionID position, List<byte> dotPoints, MirrorDirection mirrorDirection)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionID position, DotPoint[] dotPoints, MirrorDirection mirrorDirection)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionID position, List<DotPoint> dotPoints, MirrorDirection mirrorDirection)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);
        #endregion

        #region PlayMirroredDotAndPath
        public static void PlayMirrored<A>(string key, int durationMillis, PositionID position, int[] dotPoints, A pathPoints, MirrorDirection dotMirrorDirection)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection);
        public static void PlayMirrored<A>(string key, int durationMillis, PositionID position, List<int> dotPoints, A pathPoints, MirrorDirection dotMirrorDirection)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection);
        public static void PlayMirrored<A>(string key, int durationMillis, PositionID position, byte[] dotPoints, A pathPoints, MirrorDirection dotMirrorDirection)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection);
        public static void PlayMirrored<A>(string key, int durationMillis, PositionID position, List<byte> dotPoints, A pathPoints, MirrorDirection dotMirrorDirection)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection);
        public static void PlayMirrored<A>(string key, int durationMillis, PositionID position, DotPoint[] dotPoints, A pathPoints, MirrorDirection dotMirrorDirection)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection);
        public static void PlayMirrored<A>(string key, int durationMillis, PositionID position, List<DotPoint> dotPoints, A pathPoints, MirrorDirection dotMirrorDirection)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection);
        #endregion

        #region PlayRegistered
        public static void PlayRegistered(string key) 
            => Connection.PlayRegistered(key);
        public static void PlayRegistered(string key, int startTimeMillis)
            => Connection.PlayRegisteredMillis(key, startTimeMillis: startTimeMillis);
        public static void PlayRegistered(string key, ScaleOption option)
            => Connection.PlayRegistered(key, scaleOption: option);
        public static void PlayRegistered(string key, RotationOption option) 
            => Connection.PlayRegistered(key, rotationOption: option);
        public static void PlayRegistered(string key, ScaleOption scaleOption, RotationOption rotationOption) 
            => Connection.PlayRegistered(key, scaleOption: scaleOption, rotationOption: rotationOption);
        #endregion

        #region PlayRegisteredAlt
        public static void PlayRegistered(string key, string altKey) 
            => Connection.PlayRegistered(key, altKey);
        public static void PlayRegistered(string key, string altKey, ScaleOption option) 
            => Connection.PlayRegistered(key, altKey, scaleOption: option);
        public static void PlayRegistered(string key, string altKey, RotationOption option) 
            => Connection.PlayRegistered(key, altKey, rotationOption: option);
        public static void PlayRegistered(string key, string altKey, ScaleOption scaleOption, RotationOption rotationOption) 
            => Connection.PlayRegistered(key, altKey, scaleOption: scaleOption, rotationOption: rotationOption);
        #endregion
    }
}