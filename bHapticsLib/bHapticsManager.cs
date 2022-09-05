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
        /// <returns>true if Successful, otherwise false</returns>
        public static bool Connect(string id, string name, bool tryToReconnect = true, int maxRetries = 5)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if ((Status != bHapticsStatus.Disconnected) 
                && !Disconnect())
                return false;

            Connection.Setup(null, id, name, tryToReconnect, maxRetries);
            return Connection.BeginInit();
        }

        /// <summary>Disconnects from the bHaptics Player</summary>
        /// <returns>true if Successful, otherwise false</returns>
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

        /// <summary>Stops all currently playing patterns</summary>
        public static void StopPlayingAll() 
            => Connection.StopPlayingAll();
        #endregion

        #region RegisterPattern
        /// <summary>Checks if the specified pattern is Registered in the Cache</summary>
        /// <param name="key">The key id of the pattern</param>
        /// <returns>true if Registered, otherwise false</returns>
        public static bool IsPatternRegistered(string key) 
            => Connection.IsPatternRegistered(key);

        /// <summary>Registers a pattern from raw json in the cache</summary>
        /// <param name="key">The key id of the pattern</param>
        /// <param name="tactFileJson">The raw json of the pattern</param>
        public static void RegisterPatternFromJson(string key, string tactFileJson)
            => Connection.RegisterPatternFromJson(key, tactFileJson);

        /// <summary>Registers a pattern from file path in the cache</summary>
        /// <param name="key">The key id of the pattern</param>
        /// <param name="tactFilePath">The file path of the pattern</param>
        public static void RegisterPatternFromFile(string key, string tactFilePath)
            => Connection.RegisterPatternFromFile(key, tactFilePath);

        /// <summary>Registers a pattern swapped from raw json in the cache</summary>
        /// <param name="key">The key id of the pattern</param>
        /// <param name="tactFileJson">The raw json of the pattern</param>
        public static void RegisterPatternSwappedFromJson(string key, string tactFileJson)
            => Connection.RegisterPatternSwappedFromJson(key, tactFileJson);

        /// <summary>Registers a pattern swapped from file path in the cache</summary>
        /// <param name="key">The key id of the pattern</param>
        /// <param name="tactFilePath">The file path of the pattern</param>
        public static void RegisterPatternSwappedFromFile(string key, string tactFilePath)
            => Connection.RegisterPatternSwappedFromFile(key, tactFilePath);
        #endregion

        #region PlayDot
        /// <summary>Plays a pattern</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">Array of int signifying DotPoints</param>
        public static void Play(string key, int durationMillis, PositionID position, int[] dotPoints) 
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null);

        /// <summary>Plays a pattern</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">List of int signifying DotPoints</param>
        public static void Play(string key, int durationMillis, PositionID position, List<int> dotPoints)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null);

        /// <summary>Plays a pattern</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">Array of byte signifying DotPoints</param>
        public static void Play(string key, int durationMillis, PositionID position, byte[] dotPoints)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null);

        /// <summary>Plays a pattern</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">List of byte signifying DotPoints</param>
        public static void Play(string key, int durationMillis, PositionID position, List<byte> dotPoints)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null);

        /// <summary>Plays a pattern</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">Array of DotPoint</param>
        public static void Play(string key, int durationMillis, PositionID position, DotPoint[] dotPoints)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null);

        /// <summary>Plays a pattern</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">List of DotPoint</param>
        public static void Play(string key, int durationMillis, PositionID position, List<DotPoint> dotPoints)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null);
        #endregion

        #region PlayPath
        /// <summary>Plays a pattern</summary>
        /// <typeparam name="A">PathPoint Collection Type</typeparam>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="pathPoints">Collection of PathPoint</param>
        public static void Play<A>(string key, int durationMillis, PositionID position, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, (DotPoint[])null, pathPoints);
        #endregion

        #region PlayDotAndPath
        /// <summary>Plays a pattern</summary>
        /// <typeparam name="A">PathPoint Collection Type</typeparam>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">Array of int signifying DotPoints</param>
        /// <param name="pathPoints">Collection of PathPoint</param>
        public static void Play<A>(string key, int durationMillis, PositionID position, int[] dotPoints, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints);

        /// <summary>Plays a pattern</summary>
        /// <typeparam name="A">PathPoint Collection Type</typeparam>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">List of int signifying DotPoints</param>
        /// <param name="pathPoints">Collection of PathPoint</param>
        public static void Play<A>(string key, int durationMillis, PositionID position, List<int> dotPoints, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints);

        /// <summary>Plays a pattern</summary>
        /// <typeparam name="A">PathPoint Collection Type</typeparam>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">Array of byte signifying DotPoints</param>
        /// <param name="pathPoints">Collection of PathPoint</param>
        public static void Play<A>(string key, int durationMillis, PositionID position, byte[] dotPoints, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints);

        /// <summary>Plays a pattern</summary>
        /// <typeparam name="A">PathPoint Collection Type</typeparam>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">List of byte signifying DotPoints</param>
        /// <param name="pathPoints">Collection of PathPoint</param>
        public static void Play<A>(string key, int durationMillis, PositionID position, List<byte> dotPoints, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints);

        /// <summary>Plays a pattern</summary>
        /// <typeparam name="A">PathPoint Collection Type</typeparam>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">Array of DotPoint</param>
        /// <param name="pathPoints">Collection of PathPoint</param>
        public static void Play<A>(string key, int durationMillis, PositionID position, DotPoint[] dotPoints, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints);

        /// <summary>Plays a pattern</summary>
        /// <typeparam name="A">PathPoint Collection Type</typeparam>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">List of DotPoint</param>
        /// <param name="pathPoints">Collection of PathPoint</param>
        public static void Play<A>(string key, int durationMillis, PositionID position, List<DotPoint> dotPoints, A pathPoints)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints);
        #endregion

        #region PlayMirroredDot
        /// <summary>Plays a pattern mirrored</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">Array of int signifying DotPoints</param>
        /// <param name="mirrorDirection">Direction to Mirror Playback</param>
        public static void PlayMirrored(string key, int durationMillis, PositionID position, int[] dotPoints, MirrorDirection mirrorDirection)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);

        /// <summary>Plays a pattern mirrored</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">List of int signifying DotPoints</param>
        /// <param name="mirrorDirection">Direction to Mirror Playback</param>
        public static void PlayMirrored(string key, int durationMillis, PositionID position, List<int> dotPoints, MirrorDirection mirrorDirection)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);

        /// <summary>Plays a pattern mirrored</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">Array of byte signifying DotPoints</param>
        /// <param name="mirrorDirection">Direction to Mirror Playback</param>
        public static void PlayMirrored(string key, int durationMillis, PositionID position, byte[] dotPoints, MirrorDirection mirrorDirection)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);

        /// <summary>Plays a pattern mirrored</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">List of byte signifying DotPoints</param>
        /// <param name="mirrorDirection">Direction to Mirror Playback</param>
        public static void PlayMirrored(string key, int durationMillis, PositionID position, List<byte> dotPoints, MirrorDirection mirrorDirection)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);

        /// <summary>Plays a pattern mirrored</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">Array of DotPoint</param>
        /// <param name="mirrorDirection">Direction to Mirror Playback</param>
        public static void PlayMirrored(string key, int durationMillis, PositionID position, DotPoint[] dotPoints, MirrorDirection mirrorDirection)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);

        /// <summary>Plays a pattern mirrored</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">List of DotPoint</param>
        /// <param name="mirrorDirection">Direction to Mirror Playback</param>
        public static void PlayMirrored(string key, int durationMillis, PositionID position, List<DotPoint> dotPoints, MirrorDirection mirrorDirection)
            => Connection.Play(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);
        #endregion

        #region PlayMirroredDotAndPath
        /// <summary>Plays a pattern mirrored</summary>
        /// <typeparam name="A">PathPoint Collection Type</typeparam>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">Array of int signifying DotPoints</param>
        /// <param name="pathPoints">Collection of PathPoint</param>
        /// <param name="dotMirrorDirection">Direction to Mirror Playback</param>
        public static void PlayMirrored<A>(string key, int durationMillis, PositionID position, int[] dotPoints, A pathPoints, MirrorDirection dotMirrorDirection)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection);

        /// <summary>Plays a pattern mirrored</summary>
        /// <typeparam name="A">PathPoint Collection Type</typeparam>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">List of int signifying DotPoints</param>
        /// <param name="pathPoints">Collection of PathPoint</param>
        /// <param name="dotMirrorDirection">Direction to Mirror Playback</param>
        public static void PlayMirrored<A>(string key, int durationMillis, PositionID position, List<int> dotPoints, A pathPoints, MirrorDirection dotMirrorDirection)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection);

        /// <summary>Plays a pattern mirrored</summary>
        /// <typeparam name="A">PathPoint Collection Type</typeparam>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">Array of byte signifying DotPoints</param>
        /// <param name="pathPoints">Collection of PathPoint</param>
        /// <param name="dotMirrorDirection">Direction to Mirror Playback</param>
        public static void PlayMirrored<A>(string key, int durationMillis, PositionID position, byte[] dotPoints, A pathPoints, MirrorDirection dotMirrorDirection)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection);

        /// <summary>Plays a pattern mirrored</summary>
        /// <typeparam name="A">PathPoint Collection Type</typeparam>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">List of byte signifying DotPoints</param>
        /// <param name="pathPoints">Collection of PathPoint</param>
        /// <param name="dotMirrorDirection">Direction to Mirror Playback</param>
        public static void PlayMirrored<A>(string key, int durationMillis, PositionID position, List<byte> dotPoints, A pathPoints, MirrorDirection dotMirrorDirection)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection);

        /// <summary>Plays a pattern mirrored</summary>
        /// <typeparam name="A">PathPoint Collection Type</typeparam>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">Array of DotPoint</param>
        /// <param name="pathPoints">Collection of PathPoint</param>
        /// <param name="dotMirrorDirection">Direction to Mirror Playback</param>
        public static void PlayMirrored<A>(string key, int durationMillis, PositionID position, DotPoint[] dotPoints, A pathPoints, MirrorDirection dotMirrorDirection)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection);

        /// <summary>Plays a pattern mirrored</summary>
        /// <typeparam name="A">PathPoint Collection Type</typeparam>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="durationMillis">Duration of Playback</param>
        /// <param name="position">Position for Playback</param>
        /// <param name="dotPoints">List of DotPoint</param>
        /// <param name="pathPoints">Collection of PathPoint</param>
        /// <param name="dotMirrorDirection">Direction to Mirror Playback</param>
        public static void PlayMirrored<A>(string key, int durationMillis, PositionID position, List<DotPoint> dotPoints, A pathPoints, MirrorDirection dotMirrorDirection)
            where A : IList<PathPoint>, ICollection<PathPoint>
            => Connection.Play(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection);
        #endregion

        #region PlayRegistered
        /// <summary>Plays a registered pattern from the Cache</summary>
        /// <param name="key">Key id of this pattern</param>
        public static void PlayRegistered(string key) 
            => Connection.PlayRegistered(key);

        /// <summary>Plays a registered pattern from the Cache</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="startTimeMillis">Playback Start Time Delay in Milliseconds</param>
        public static void PlayRegistered(string key, int startTimeMillis)
            => Connection.PlayRegisteredMillis(key, startTimeMillis: startTimeMillis);

        /// <summary>Plays a registered pattern from the Cache</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="option">Custom Playback Scale Option, can be null</param>
        public static void PlayRegistered(string key, ScaleOption option)
            => Connection.PlayRegistered(key, scaleOption: option);

        /// <summary>Plays a registered pattern from the Cache</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="option">Custom Playback Rotation Option, can be null</param>
        public static void PlayRegistered(string key, RotationOption option) 
            => Connection.PlayRegistered(key, rotationOption: option);

        /// <summary>Plays a registered pattern from the Cache</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="scaleOption">Custom Playback Scale Option, can be null</param>
        /// <param name="rotationOption">Custom Playback Rotation Option, can be null</param>
        public static void PlayRegistered(string key, ScaleOption scaleOption, RotationOption rotationOption) 
            => Connection.PlayRegistered(key, scaleOption: scaleOption, rotationOption: rotationOption);
        #endregion

        #region PlayRegisteredAlt
        /// <summary>Plays a registered pattern from the Cache</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="altKey">Alternative Key id of this pattern, can be null</param>
        public static void PlayRegistered(string key, string altKey) 
            => Connection.PlayRegistered(key, altKey);

        /// <summary>Plays a registered pattern from the Cache</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="altKey">Alternative Key id of this pattern, can be null</param>
        /// <param name="option">Custom Playback Scale Option, can be null</param>
        public static void PlayRegistered(string key, string altKey, ScaleOption option) 
            => Connection.PlayRegistered(key, altKey, scaleOption: option);

        /// <summary>Plays a registered pattern from the Cache</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="altKey">Alternative Key id of this pattern, can be null</param>
        /// <param name="option">Custom Playback Rotation Option, can be null</param>
        public static void PlayRegistered(string key, string altKey, RotationOption option) 
            => Connection.PlayRegistered(key, altKey, rotationOption: option);

        /// <summary>Plays a registered pattern from the Cache</summary>
        /// <param name="key">Key id of this pattern</param>
        /// <param name="altKey">Alternative Key id of this pattern, can be null</param>
        /// <param name="scaleOption">Custom Playback Scale Option, can be null</param>
        /// <param name="rotationOption">Custom Playback Rotation Option, can be null</param>
        public static void PlayRegistered(string key, string altKey, ScaleOption scaleOption, RotationOption rotationOption) 
            => Connection.PlayRegistered(key, altKey, scaleOption: scaleOption, rotationOption: rotationOption);
        #endregion
    }
}