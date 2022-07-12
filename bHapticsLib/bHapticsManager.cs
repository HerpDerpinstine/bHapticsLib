using System.Collections.Generic;
using System.Net;
using bHapticsLib.Internal.Connection;

namespace bHapticsLib
{
    public static class bHapticsManager
    {
        public static readonly int MaxIntensity = 500;
        public static readonly int MaxMotorsPerDotPoint = 20;
        public static readonly int MaxMotorsPerPathPoint = 3;

        public static IPAddress IPAddress = IPAddress.Loopback;
        internal static int Port = 15881;
        internal static string Endpoint = "v2/feedbacks";

        private static ConnectionManager Connection = new ConnectionManager();

        public static bool Initialize(string id, string name, bool tryToReconnect = true, int maxRetries = 5)
        {
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

        public static bool Quit() => Connection.EndInit();

        public static bool IsInitialized() => Connection.IsAlive();

        public static bool IsPlayerConnected() => Connection.IsPlayerConnected();

        public static int GetConnectedDeviceCount() => Connection.GetConnectedDeviceCount();
        public static bool IsAnyDevicesConnected() => GetConnectedDeviceCount() > 0;
        public static bool IsDeviceConnected(PositionType type) => Connection.IsDeviceConnected(type);

        public static int[] GetDeviceStatus(PositionType type) => Connection.GetDeviceStatus(type);
        public static bool IsAnyMotorActive(PositionType type) => GetDeviceStatus(type)?.ContainsValueMoreThan(0) ?? false;

        public static bool IsPlaying(string key) => Connection.IsPlaying(key);
        public static bool IsPlayingAny() => Connection.IsPlayingAny();

        public static void StopPlaying(string key) => Connection.StopPlaying(key);
        public static void StopPlayingAll() => Connection.StopPlayingAll();

        public static bool IsFeedbackRegistered(string key) => Connection.IsFeedbackRegistered(key);

        public static void RegisterFeedbackFromJson(string key, string tactFileStr) => Connection.RegisterFeedbackFromJson(key, tactFileStr);
        //public static void RegisterFeedbackFromJsonReflected(string key, string tactFileStr) => Connection.RegisterFeedbackFromJsonReflected(key, tactFileStr);

        public static void RegisterFeedbackFromFile(string key, string tactFilePath) => Connection.RegisterFeedbackFromFile(key, tactFilePath);
        //public static void RegisterFeedbackFromFileReflected(string key, string tactFilePath) => Connection.RegisterFeedbackFromFileReflected(key, tactFilePath);

        //public static void Submit(string key, int durationMillis, PositionType position, int[] dotPoints)
        //public static void Submit(string key, int durationMillis, PositionType position, byte[] dotPoints)

        public static void Submit(string key, int durationMillis, PositionType position, DotPoint[] dotPoints) => Connection.Submit(key, durationMillis, position, dotPoints, null);
        public static void Submit(string key, int durationMillis, PositionType position, PathPoint[] pathPoints) => Connection.Submit(key, durationMillis, position, null, pathPoints);
        public static void Submit(string key, int durationMillis, PositionType position, DotPoint[] dotPoints, PathPoint[] pathPoints) => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints);

        public static void Submit(string key, int durationMillis, PositionType position, List<DotPoint> dotPoints) => Connection.Submit(key, durationMillis, position, dotPoints, null);
        public static void Submit(string key, int durationMillis, PositionType position, List<PathPoint> pathPoints) => Connection.Submit(key, durationMillis, position, null, pathPoints);
        public static void Submit(string key, int durationMillis, PositionType position, List<DotPoint> dotPoints, List<PathPoint> pathPoints) => Connection.Submit(key, durationMillis, position, dotPoints, pathPoints);

        public static void SubmitRegistered(string key) => Connection.SubmitRegistered(key);
        public static void SubmitRegistered(string key, ScaleOption option) => Connection.SubmitRegistered(key, scaleOption: option);
        public static void SubmitRegistered(string key, RotationOption option) => Connection.SubmitRegistered(key, rotationOption: option);
        public static void SubmitRegistered(string key, ScaleOption scaleOption, RotationOption rotationOption) => Connection.SubmitRegistered(key, null, scaleOption, rotationOption);

        public static void SubmitRegistered(string key, string altKey) => Connection.SubmitRegistered(key, altKey);
        public static void SubmitRegistered(string key, string altKey, ScaleOption option) => Connection.SubmitRegistered(key, altKey, scaleOption: option);
        public static void SubmitRegistered(string key, string altKey, RotationOption option) => Connection.SubmitRegistered(key, altKey, rotationOption: option);
        public static void SubmitRegistered(string key, string altKey, ScaleOption scaleOption, RotationOption rotationOption) => Connection.SubmitRegistered(key, altKey, scaleOption, rotationOption);
    }
}