using System.Collections.Generic;

namespace bHapticsLib
{
    /// <summary>AIO bHaptics Management</summary>
    public static class bHapticsManager
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
        public static bHapticsStatus ConnectionStatus { get => Connection.Status; }

        public static bool Connect(string id, string name, bool tryToReconnect = true, int maxRetries = 5)
        {
            if (string.IsNullOrEmpty(id))
                return false; // To-Do: Throw Exception

            if (string.IsNullOrEmpty(name))
                return false; // To-Do: Throw Exception

            if (ConnectionStatus != bHapticsStatus.Disconnected)
                Disconnect();

            Connection.ID = id;
            Connection.Name = name;
            Connection.TryToReconnect = tryToReconnect;
            Connection.MaxRetries = maxRetries.Clamp(0, int.MaxValue);

            return Connection.BeginInit();
        }

        public static bool Disconnect()
        {
            if (ConnectionStatus == bHapticsStatus.Disconnected)
                return true;

            StopPlayingAll();
            return Connection.EndInit();
        }
        #endregion

        #region Device
        public static int GetConnectedDeviceCount() 
            => Connection.GetConnectedDeviceCount();
        public static bool IsAnyDevicesConnected() 
            => GetConnectedDeviceCount() > 0;
        public static bool IsDeviceConnected(PositionID type) 
            => Connection.IsDeviceConnected(type);

        public static int[] GetDeviceStatus(PositionID type)
            => Connection.GetDeviceStatus(type);
        public static bool IsAnyMotorActive(PositionID type)
            => GetDeviceStatus(type)?.ContainsValueMoreThan(0) ?? false;
#endregion

        #region IsPlaying
        public static bool IsPlaying(string key) 
            => Connection.IsPlaying(key);
        public static bool IsPlayingAny()
            => Connection.IsPlayingAny();
        #endregion

        #region StopPlaying
        public static void StopPlaying(string key) 
            => Connection.StopPlaying(key);
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