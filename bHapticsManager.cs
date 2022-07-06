using System.Collections.Generic;
using bHapticsLib.Internal.Connection;

namespace bHapticsLib
{
    public static class bHapticsManager
    {
        public const int MaxIntensity = 500;
        public const int MaxBufferSize = 20;

        private static RequestManager Request;

        public static void Initialize(string id, string name, bool tryreconnect = true)
        {
            // To-Do: Null Check and Throw Exception
            // id
            // name

            //if (!ExePathCheck()
            //    && !SteamLibraryCheck())
            //    throw new Exception("bHaptics Player is Not Installed!");

            if (Request != null)
                Request.EndInit();
            else
                Request = new RequestManager(id, name);

            Request.TryReconnect = tryreconnect;
            Request.BeginInit();
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
            Request?.StopPlayingAll();
            Request?.EndInit();
        }

        public static bool IsConnected() => Request?.IsConnected() ?? false;

        public static bool IsPlaying(string key) => Request?.IsPlaying(key) ?? false;
        public static bool IsPlaying(PositionType type) => Request?.IsPlaying(type) ?? false;
        public static bool IsPlayingAny() => Request?.IsPlayingAny() ?? false;

        public static void StopPlaying(string key) => Request?.StopPlaying(key);
        public static void StopPlayingAll() => Request?.StopPlayingAll();

        public static void RegisterFeedbackFromTactFile(string key, string tactFileStr) => Request?.RegisterTactFileStr(key, tactFileStr);

        public static void Submit(string key, PositionType position, byte[] bytes, int durationMillis) => Request?.Submit(key, position, bytes, durationMillis);
        public static void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis) => Request?.Submit(key, position, points, durationMillis);
        public static void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis) => Request?.Submit(key, position, points, durationMillis);

        public static void SubmitRegistered(string key) => Request?.SubmitRegistered(key);
        public static void SubmitRegistered(string key, int startTimeMillis) => Request?.SubmitRegistered(key, startTimeMillis);
        public static void SubmitRegistered(string key, string altKey, ScaleOption option) => Request?.SubmitRegistered(key, altKey, option);
        public static void SubmitRegistered(string key, string altKey, ScaleOption sOption, RotationOption rOption) => Request?.SubmitRegisteredVestRotation(key, altKey, rOption, sOption);

        public static string PositionTypeToOscAddress(PositionType positionType)
        {
            switch (positionType)
            {
                // Head
                case PositionType.Head:
                    return "/head";

                // Vest
                case PositionType.Vest:
                    return "/vest";
                case PositionType.VestFront:
                    return "/vest/front";
                case PositionType.VestBack:
                    return "/vest/back";

                // Arms
                case PositionType.ForearmL:
                    return "/arm/left";
                case PositionType.ForearmR:
                    return "/arm/right";

                // Hands
                case PositionType.HandL:
                    return "/hand/left";
                case PositionType.HandR:
                    return "/hand/right";

                // Feet
                case PositionType.FootL:
                    return "/foot/left";
                case PositionType.FootR:
                    return "/foot/right";

                // Unknown
                default:
                    return "/unknown";
            }
        }
    }
}