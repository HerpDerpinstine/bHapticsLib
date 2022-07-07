using System.Collections.Generic;
using bHapticsLib.Internal.Connection;

namespace bHapticsLib
{
    public static class bHapticsManager
    {
        public const int MaxIntensity = 500;
        public const int MaxMotorCount = 3;
        public const int MaxBufferSize = 20;

        private static ConnectionManager Connection = new ConnectionManager();

        public static void Initialize(string id, string name, bool tryreconnect = true)
        {
            // To-Do: Null Check and Throw Exception
            // id
            // name

            //if (!ExePathCheck()
            //    && !SteamLibraryCheck())
            //    throw new Exception("bHaptics Player is Not Installed!");

            Connection.EndInit();

            Connection.ID = id;
            Connection.Name = name;
            Connection.TryReconnect = tryreconnect;

            Connection.BeginInit();
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

        public static void Quit()
        {
            Connection.StopPlayingAll();
            Connection.EndInit();
        }

        public static bool IsConnected() => Connection.IsConnected();
        public static bool IsDeviceConnected(PositionType type) => Connection.IsDeviceConnected(type);
        public static bool IsAnyDeviceConnected() => Connection.IsAnyDeviceConnected();

        public static bool IsPlaying(string key) => Connection.IsPlaying(key);
        //public static bool IsPlaying(PositionType type) => Connection.IsPlaying(type);
        public static bool IsPlayingAny() => Connection.IsPlayingAny();

        public static void StopPlaying(string key) => Connection.StopPlaying(key);
        public static void StopPlayingAll() => Connection.StopPlayingAll();

        public static bool IsFeedbackRegistered(string key) => Connection.IsFeedbackRegistered(key);

        public static void RegisterFeedback(string key, string tactFileStr) { if (!_waserror) NativeLib.RegisterFeedback(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(tactFileStr)); }
        public static void RegisterFeedbackFromTactFile(string key, string tactFileStr) { if (!_waserror) NativeLib.RegisterFeedbackFromTactFile(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(tactFileStr)); }
        public static void RegisterFeedbackFromTactFileReflected(string key, string tactFileStr) { if (!_waserror) NativeLib.RegisterFeedbackFromTactFileReflected(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(tactFileStr)); }

        public static void Submit(string key, PositionType position, byte[] bytes, int durationMillis) => Connection.Submit(key, position, bytes, durationMillis);
        public static void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis) => Connection.Submit(key, position, points, durationMillis);
        public static void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis) => Connection.Submit(key, position, points, durationMillis);

        public static void SubmitRegistered(string key) { if (!_waserror) NativeLib.SubmitRegistered(Marshal.StringToHGlobalAnsi(key)); }
        public static void SubmitRegistered(string key, int startTimeMillis) => NativeLib.SubmitRegisteredStartMillis(Marshal.StringToHGlobalAnsi(key), startTimeMillis);

        public static void SubmitRegistered(string key, string altKey, ScaleOption option) { if (!_waserror) NativeLib.SubmitRegisteredWithOption(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(altKey), option.Intensity, option.Duration, 1f, 1f); }
        public static void SubmitRegistered(string key, string altKey, RotationOption option) { if (!_waserror) NativeLib.SubmitRegisteredWithOption(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(altKey), option.Intensity, option.Duration, 1f, 1f); }
        public static void SubmitRegistered(string key, string altKey, ScaleOption sOption, RotationOption rOption) { if (!_waserror) NativeLib.SubmitRegisteredWithOption(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(altKey), sOption.Intensity, sOption.Duration, rOption.OffsetX, rOption.OffsetY); }
    }
}