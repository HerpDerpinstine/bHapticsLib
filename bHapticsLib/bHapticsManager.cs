using System.Collections.Generic;
using System.Net;
using bHapticsLib.Internal.Connection;

namespace bHapticsLib
{
    public static class bHapticsManager
    {
        #region MaxValues
        public static readonly int MaxIntensity = 500;
        public static readonly int MaxMotorsPerDotPoint = 20;
        public static readonly int MaxMotorsPerPathPoint = 3;
        #endregion

        #region Connection
        public static IPAddress IPAddress = IPAddress.Loopback;
        internal static int Port = 15881;
        internal static string Endpoint = "v2/feedbacks";

        private static ConnectionManager Connection = new ConnectionManager();

        public static bHapticsStatus ConnectionStatus
        {
            get => !Connection.IsAlive() ? bHapticsStatus.Disconnected 
                : !Connection.IsConnected() ? bHapticsStatus.Connecting
                : bHapticsStatus.Connected;
        }

        public static bool Connect(string id, string name, bool tryToReconnect = true, int maxRetries = 5)
        {
            if (ConnectionStatus != bHapticsStatus.Disconnected)
                return false; // To-Do: Throw Exception

            if (string.IsNullOrEmpty(id))
                return false; // To-Do: Throw Exception

            if (string.IsNullOrEmpty(name))
                return false; // To-Do: Throw Exception

            Connection.ID = id;
            Connection.Name = name;
            Connection.TryToReconnect = tryToReconnect;
            Connection.MaxRetries = maxRetries.Clamp(0, int.MaxValue);

            return Connection.BeginInit();
        }

        public static bool Disconnect()
        {
            if (ConnectionStatus == bHapticsStatus.Disconnected)
                return false; // To-Do: Throw Exception

            return Connection.EndInit();
        }
        #endregion

        #region Device
        public static int GetConnectedDeviceCount() 
            => Connection.GetConnectedDeviceCount();
        public static bool IsAnyDevicesConnected() 
            => GetConnectedDeviceCount() > 0;
        public static bool IsDeviceConnected(PositionType type) 
            => Connection.IsDeviceConnected(type);

        public static int[] GetDeviceStatus(PositionType type)
            => Connection.GetDeviceStatus(type);
        public static bool IsAnyMotorActive(PositionType type)
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
        #endregion

        #region RegisterPatternMirrored
        //public static void RegisterPatternMirroredFromJson(string key, string tactFileStr, MirrorDirection mirrorDirection)
        //  => Connection.RegisterPatternMirroredFromJson(key, tactFileStr, mirrorDirection);
        //public static void RegisterPatternMirroredFromFile(string key, string tactFilePath, MirrorDirection mirrorDirection)
        //  => Connection.RegisterPatternMirroredFromFile(key, tactFilePath, mirrorDirection);
        #endregion

        #region PlayDot
        public static void Play(string key, int durationMillis, PositionType position, int[] dotPoints) 
            => Connection.Submit(key, durationMillis, position, dotPoints, (PathPoint[])null);
        public static void Play(string key, int durationMillis, PositionType position, List<int> dotPoints)
            => Connection.Submit(key, durationMillis, position, dotPoints, (PathPoint[])null);
        public static void Play(string key, int durationMillis, PositionType position, byte[] dotPoints)
            => Connection.Submit(key, durationMillis, position, dotPoints, (PathPoint[])null);
        public static void Play(string key, int durationMillis, PositionType position, List<byte> dotPoints)
            => Connection.Submit(key, durationMillis, position, dotPoints, (PathPoint[])null);
        public static void Play(string key, int durationMillis, PositionType position, DotPoint[] dotPoints)
            => Connection.Submit(key, durationMillis, position, dotPoints, (PathPoint[])null);
        public static void Play(string key, int durationMillis, PositionType position, List<DotPoint> dotPoints)
            => Connection.Submit(key, durationMillis, position, dotPoints, (PathPoint[])null);
        #endregion

        #region PlayPath
        public static void Play(string key, int durationMillis, PositionType position, PathPoint[] pathPoints)
            => Connection.Submit(key, durationMillis, position, (DotPoint[])null, pathPoints);
        public static void Play(string key, int durationMillis, PositionType position, List<PathPoint> pathPoints)
            => Connection.Submit(key, durationMillis, position, (DotPoint[])null, pathPoints);
        #endregion

        #region PlayDotAndPath
        public static void Play(string key, int durationMillis, PositionType position, int[] dotPoints, PathPoint[] pathPoints)
            => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints);
        public static void Play(string key, int durationMillis, PositionType position, List<int> dotPoints, PathPoint[] pathPoints)
            => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints);
        public static void Play(string key, int durationMillis, PositionType position, byte[] dotPoints, PathPoint[] pathPoints)
            => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints);
        public static void Play(string key, int durationMillis, PositionType position, List<byte> dotPoints, PathPoint[] pathPoints)
            => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints);
        public static void Play(string key, int durationMillis, PositionType position, DotPoint[] dotPoints, PathPoint[] pathPoints)
            => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints);
        public static void Play(string key, int durationMillis, PositionType position, List<DotPoint> dotPoints, List<PathPoint> pathPoints)
            => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints);
        #endregion

        #region PlayMirroredDot
        public static void PlayMirrored(string key, int durationMillis, PositionType position, int[] dotPoints, MirrorDirection mirrorDirection)
            => Connection.Submit(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionType position, List<int> dotPoints, MirrorDirection mirrorDirection)
            => Connection.Submit(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionType position, byte[] dotPoints, MirrorDirection mirrorDirection)
            => Connection.Submit(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionType position, List<byte> dotPoints, MirrorDirection mirrorDirection)
            => Connection.Submit(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionType position, DotPoint[] dotPoints, MirrorDirection mirrorDirection)
            => Connection.Submit(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionType position, List<DotPoint> dotPoints, MirrorDirection mirrorDirection)
            => Connection.Submit(key, durationMillis, position, dotPoints, (PathPoint[])null, mirrorDirection);
        #endregion

        #region PlayMirroredPath
        public static void PlayMirrored(string key, int durationMillis, PositionType position, PathPoint[] pathPoints, MirrorDirection mirrorDirection)
            => Connection.Submit(key, durationMillis, position, (DotPoint[])null, pathPoints, MirrorDirection.None, mirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionType position, List<PathPoint> pathPoints, MirrorDirection mirrorDirection)
            => Connection.Submit(key, durationMillis, position, (DotPoint[])null, pathPoints, MirrorDirection.None, mirrorDirection);
        #endregion

        #region PlayMirroredDotAndPath
        public static void PlayMirrored(string key, int durationMillis, PositionType position, int[] dotPoints, PathPoint[] pathPoints, MirrorDirection dotMirrorDirection, MirrorDirection pathMirrorDirection)
            => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection, pathMirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionType position, List<int> dotPoints, PathPoint[] pathPoints, MirrorDirection dotMirrorDirection, MirrorDirection pathMirrorDirection)
            => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection, pathMirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionType position, byte[] dotPoints, PathPoint[] pathPoints, MirrorDirection dotMirrorDirection, MirrorDirection pathMirrorDirection)
            => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection, pathMirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionType position, List<byte> dotPoints, PathPoint[] pathPoints, MirrorDirection dotMirrorDirection, MirrorDirection pathMirrorDirection)
            => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection, pathMirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionType position, DotPoint[] dotPoints, PathPoint[] pathPoints, MirrorDirection dotMirrorDirection, MirrorDirection pathMirrorDirection)
            => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection, pathMirrorDirection);
        public static void PlayMirrored(string key, int durationMillis, PositionType position, List<DotPoint> dotPoints, List<PathPoint> pathPoints, MirrorDirection dotMirrorDirection, MirrorDirection pathMirrorDirection)
            => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints, dotMirrorDirection, pathMirrorDirection);
        #endregion

        #region PlayRegistered
        public static void PlayRegistered(string key) 
            => Connection.SubmitRegistered(key);
        public static void PlayRegistered(string key, ScaleOption option)
            => Connection.SubmitRegistered(key, scaleOption: option);
        public static void PlayRegistered(string key, RotationOption option) 
            => Connection.SubmitRegistered(key, rotationOption: option);
        public static void PlayRegistered(string key, ScaleOption scaleOption, RotationOption rotationOption) 
            => Connection.SubmitRegistered(key, null, scaleOption, rotationOption);
        #endregion

        #region PlayRegisteredAlt
        public static void PlayRegistered(string key, string altKey) 
            => Connection.SubmitRegistered(key, altKey);
        public static void PlayRegistered(string key, string altKey, ScaleOption option) 
            => Connection.SubmitRegistered(key, altKey, scaleOption: option);
        public static void PlayRegistered(string key, string altKey, RotationOption option) 
            => Connection.SubmitRegistered(key, altKey, rotationOption: option);
        public static void PlayRegistered(string key, string altKey, ScaleOption scaleOption, RotationOption rotationOption) 
            => Connection.SubmitRegistered(key, altKey, scaleOption, rotationOption);
        #endregion
    }
}