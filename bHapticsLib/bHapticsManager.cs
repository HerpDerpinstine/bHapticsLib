using System.Collections.Generic;
using System.Net;
using bHapticsLib.Internal.Connection;

namespace bHapticsLib
{
    public static class bHapticsManager
    {
        public static readonly int MaxIntensity = 500;
        public static readonly int MaxMotorsPerPathPoint = 3;
        public static readonly int MaxMotorsPerPositionType = 20;

        public static IPAddress IPAddress = IPAddress.Loopback;
        internal static int Port = 15881;
        internal static string Endpoint = "v2/feedbacks";

        private static ConnectionManager Connection = new ConnectionManager();
        private static string PlayerPath = null;

        public static bool Initialize(string id, string name, bool tryReconnect = true, int maxRetries = 5)
        {
            if (string.IsNullOrEmpty(id))
                return false; // To-Do: Throw Exception

            if (string.IsNullOrEmpty(name))
                return false; // To-Do: Throw Exception

            //if (!ExePathCheck()
            //    && !SteamLibraryCheck())
            //    throw new Exception("bHaptics Player is Not Installed!");

            Connection.ID = id;
            Connection.Name = name;
            Connection.TryReconnect = tryReconnect;
            Connection.MaxRetries = maxRetries.Clamp(0, int.MaxValue);

            return Connection.BeginInit();
        }

        /*
        private static bool ExePathCheck()
        {
            byte[] buf = new byte[500];
            int size = 0;
            return NativeLib?.TryGetExePath(buf, ref size) ?? false;
            return true;
        }

        private static bool SteamLibraryCheck()
            => !string.IsNullOrEmpty(SteamManifestReader.GetInstallPathFromAppId("1573010"));
        */

        public static bool Quit() => Connection.EndInit();

        public static bool IsInitialized() => Connection.IsAlive();

        public static string GetPlayerExecutablePath() => PlayerPath;
        public static bool IsPlayerConnected() => Connection.IsPlayerConnected();

        public static int GetConnectedDeviceCount() => Connection.GetConnectedDeviceCount();
        public static bool IsAnyDevicesConnected() => GetConnectedDeviceCount() > 0;
        public static bool IsDeviceConnected(PositionType type) => Connection.IsDeviceConnected(type);
        public static int[] GetDeviceStatus(PositionType type) => Connection.GetDeviceStatus(type);

        public static bool IsPlaying(string key) => Connection.IsPlaying(key);

        public static bool IsPlayingAny() => Connection.IsPlayingAny();
        public static bool IsPlayingAny(PositionType type) => GetDeviceStatus(type)?.ContainsValueMoreThan(0) ?? false;

        public static void StopPlaying(string key) => Connection.StopPlaying(key);
        public static void StopPlayingAll() => Connection.StopPlayingAll();

        public static bool IsFeedbackRegistered(string key) => Connection.IsFeedbackRegistered(key);

        /*
        public static void RegisterFeedback(string key, string tactFileStr) => Connection.RegisterFeedback(key, tactFileStr);
        public static void RegisterFeedbackFromTactFile(string key, string tactFileStr)
        public static void RegisterFeedbackFromTactFileReflected(string key, string tactFileStr)
        */

        //public static void Submit(string key, int durationMillis, PositionType position, int[] dotPoints)

        public static void Submit(string key, int durationMillis, PositionType position, DotPoint[] dotPoints) => Connection.Submit(key, durationMillis, position, dotPoints, null);
        public static void Submit(string key, int durationMillis, PositionType position, List<DotPoint> dotPoints) => Connection.Submit(key, durationMillis, position, dotPoints, null);

        public static void Submit(string key, int durationMillis, PositionType position, PathPoint[] pathPoints) => Connection.Submit(key, durationMillis, position, null, pathPoints);
        public static void Submit(string key, int durationMillis, PositionType position, List<PathPoint> pathPoints) => Connection.Submit(key, durationMillis, position, null, pathPoints);

        public static void Submit(string key, int durationMillis, PositionType position, DotPoint[] dotPoints, PathPoint[] pathPoints) => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints);
        public static void Submit(string key, int durationMillis, PositionType position, List<DotPoint> dotPoints, List<PathPoint> pathPoints) => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints);

        public static void SubmitRegistered(string key) => Connection.SubmitRegistered(key);
        public static void SubmitRegistered(string key, ScaleOption option) => Connection.SubmitRegistered(key, key, option, null);
        public static void SubmitRegistered(string key, RotationOption option) => Connection.SubmitRegistered(key, key, null, option);
        public static void SubmitRegistered(string key, ScaleOption scaleOption, RotationOption rotationOption) => Connection.SubmitRegistered(key, key, scaleOption, rotationOption);

        public static void SubmitRegistered(string key, string altKey) => Connection.SubmitRegistered(key, altKey, null, null);
        public static void SubmitRegistered(string key, string altKey, ScaleOption option) => Connection.SubmitRegistered(key, altKey, option, null);
        public static void SubmitRegistered(string key, string altKey, RotationOption option) => Connection.SubmitRegistered(key, altKey, null, option);
        public static void SubmitRegistered(string key, string altKey, ScaleOption scaleOption, RotationOption rotationOption) => Connection.SubmitRegistered(key, altKey, scaleOption, rotationOption);

        //public static void SubmitRegistered(string key, int startTimeMillis)
    }
}