using bHapticsLib.Internal.Connection;
using System.Collections.Generic;

namespace bHapticsLib
{
    public static class bHapticsManager
    {
        public const int MaxIntensity = 500;
        public const int MaxMotorCount = 3;
        public const int MaxBufferSize = 20;

        public static bool Debug = false;

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
            Connection.MaxRetries = maxRetries;

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

        public static bool IsPlaying(string key) => Connection.IsPlaying(key);
        //public static bool IsPlaying(PositionType type) => Connection.IsPlaying(type);
        public static bool IsPlayingAny() => Connection.IsPlayingAny();

        public static void StopPlaying(string key) => Connection.StopPlaying(key);
        public static void StopPlayingAll() => Connection.StopPlayingAll();

        public static bool IsFeedbackRegistered(string key) => Connection.IsFeedbackRegistered(key);

        //public static void Submit(string key, int durationMillis, PositionType position, byte[] rawBytes) => Connection.Submit(key, durationMillis, position, rawBytes);

        public static void Submit(string key, int durationMillis, PositionType position, List<DotPoint> dotPoints) => Submit(key, durationMillis, position, dotPoints, null);
        public static void Submit(string key, int durationMillis, PositionType position, List<PathPoint> pathPoints) => Submit(key, durationMillis, position, null, pathPoints);
        public static void Submit(string key, int durationMillis, PositionType position, List<DotPoint> dotPoints, List<PathPoint> pathPoints) => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints);

        /*
        public static void RegisterFeedback(string key, string tactFileStr) => Connection.RegisterFeedback(key, tactFileStr);

        public static void RegisterFeedbackFromTactFile(string key, string tactFileStr) { if (!_waserror) NativeLib.RegisterFeedbackFromTactFile(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(tactFileStr)); }
        public static void RegisterFeedbackFromTactFileReflected(string key, string tactFileStr) { if (!_waserror) NativeLib.RegisterFeedbackFromTactFileReflected(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(tactFileStr)); }
        */

        /*
        public static void Submit(string key, PositionType position, byte[] bytes, int durationMillis) => Connection.Submit(key, position, bytes, durationMillis);
        public static void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis) => Connection.Submit(key, position, points, durationMillis);
        public static void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis) => Connection.Submit(key, position, points, durationMillis);

        public static void SubmitRegistered(string key) { if (!_waserror) NativeLib.SubmitRegistered(Marshal.StringToHGlobalAnsi(key)); }
        public static void SubmitRegistered(string key, int startTimeMillis) => NativeLib.SubmitRegisteredStartMillis(Marshal.StringToHGlobalAnsi(key), startTimeMillis);

        public static void SubmitRegistered(string key, string altKey, ScaleOption option) { if (!_waserror) NativeLib.SubmitRegisteredWithOption(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(altKey), option.Intensity, option.Duration, 1f, 1f); }
        public static void SubmitRegistered(string key, string altKey, RotationOption option) { if (!_waserror) NativeLib.SubmitRegisteredWithOption(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(altKey), option.Intensity, option.Duration, 1f, 1f); }
        public static void SubmitRegistered(string key, string altKey, ScaleOption sOption, RotationOption rOption) { if (!_waserror) NativeLib.SubmitRegisteredWithOption(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(altKey), sOption.Intensity, sOption.Duration, rOption.OffsetX, rOption.OffsetY); }
        */
    }
}