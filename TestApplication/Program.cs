using bHapticsLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace TestApplication
{
    public class Program
    {
        private static HapticPattern testFeedback;

        private static void Main()
        {
            string testFeedbackPath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "testfeedback.tact");
            testFeedback = HapticPattern.LoadFromFile("testfeedback", testFeedbackPath);

            Console.WriteLine("Initializing...");
            bHapticsManager.Connect("bHapticsLib", "TestApplication");
            Console.WriteLine();

            Console.WriteLine($"Press 0 for {nameof(bHapticsManager.ConnectionStatus)}");
            Console.WriteLine();

            Console.WriteLine($"Press 2 for {nameof(bHapticsManager.GetConnectedDeviceCount)}()");
            Console.WriteLine($"Press 3 for {nameof(bHapticsManager.IsAnyDevicesConnected)}()");
            Console.WriteLine();

            Console.WriteLine($"Press 4 for {nameof(bHapticsManager.IsPlayingAny)}()");
            Console.WriteLine($"Press 5 for {nameof(bHapticsManager.IsAnyMotorActive)}({nameof(PositionType)}.{nameof(PositionType.Vest)})");
            Console.WriteLine($"Press 6 for {nameof(bHapticsManager.IsAnyMotorActive)}({nameof(PositionType)}.{nameof(PositionType.VestFront)})");
            Console.WriteLine($"Press 7 for {nameof(bHapticsManager.IsAnyMotorActive)}({nameof(PositionType)}.{nameof(PositionType.VestBack)})");
            Console.WriteLine();

            Console.WriteLine($"Press I for {nameof(bHapticsManager.GetDeviceStatus)}({nameof(PositionType)}.{nameof(PositionType.Vest)})");
            Console.WriteLine($"Press O for {nameof(bHapticsManager.GetDeviceStatus)}({nameof(PositionType)}.{nameof(PositionType.VestFront)})");
            Console.WriteLine($"Press P for {nameof(bHapticsManager.GetDeviceStatus)}({nameof(PositionType)}.{nameof(PositionType.VestBack)})");
            Console.WriteLine();

            Console.WriteLine($"Press NUMPAD-1 for {nameof(bHapticsManager.IsDeviceConnected)}({nameof(PositionType)}.{nameof(PositionType.Vest)})");
            Console.WriteLine();

            Console.WriteLine($"Press NUMPAD-2 for {nameof(testFeedback)}.{nameof(testFeedback.Play)}()");
            Console.WriteLine();


            Console.WriteLine($"Press NUMPAD-4 for {nameof(testFeedback)}.{nameof(testFeedback.IsRegistered)}()");
            Console.WriteLine();

            Console.WriteLine($"Press NUMPAD-7 for {nameof(testFeedback)}.{nameof(testFeedback.IsPlaying)}()");
            Console.WriteLine();

            Console.WriteLine($"Press NUMPAD-8 for {nameof(bHapticsManager.Play)}(\"testPlayFront\", 1000, {nameof(PositionType)}.{nameof(PositionType.VestFront)}, [ new {nameof(DotPoint)} ( index = 0, intensity = 100 ) ] )");
            Console.WriteLine($"Press NUMPAD-9 for {nameof(bHapticsManager.Play)}(\"testPlayBack\", 1000, {nameof(PositionType)}.{nameof(PositionType.VestBack)}, [ new {nameof(DotPoint)} ( index = 0, intensity = 100 ) ] )");
            Console.WriteLine();

            Console.WriteLine("Press Enter to Disconnect.");
            Console.WriteLine();

            while (true)
            {
                if (KeyCheck())
                    break;
                else
                    Thread.Sleep(1);
            }

            if (!bHapticsManager.Disconnect())
                Console.WriteLine("Failed to Disconnect!");
        }

        static bool KeyCheck()
        {
            if (!Console.KeyAvailable)
                return false;

            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    return true;


                case ConsoleKey.D0:
                    Console.WriteLine($"{nameof(bHapticsManager.ConnectionStatus)}: {bHapticsManager.ConnectionStatus}");
                    goto default;


                case ConsoleKey.D2:
                    Console.WriteLine($"{nameof(bHapticsManager.GetConnectedDeviceCount)}(): {bHapticsManager.GetConnectedDeviceCount()}");
                    goto default;
                case ConsoleKey.D3:
                    Console.WriteLine($"{nameof(bHapticsManager.IsAnyDevicesConnected)}(): {bHapticsManager.IsAnyDevicesConnected()}");
                    goto default;


                case ConsoleKey.D4:
                    Console.WriteLine($"{nameof(bHapticsManager.IsPlayingAny)}(): {bHapticsManager.IsPlayingAny()}");
                    goto default;
                case ConsoleKey.D5:
                    Console.WriteLine($"{nameof(bHapticsManager.IsAnyMotorActive)}({nameof(PositionType)}.{nameof(PositionType.Vest)}): {bHapticsManager.IsAnyMotorActive(PositionType.Vest)}");
                    goto default;
                case ConsoleKey.D6:
                    Console.WriteLine($"{nameof(bHapticsManager.IsAnyMotorActive)}({nameof(PositionType)}.{nameof(PositionType.VestFront)}): {bHapticsManager.IsAnyMotorActive(PositionType.VestFront)}");
                    goto default;
                case ConsoleKey.D7:
                    Console.WriteLine($"{nameof(bHapticsManager.IsAnyMotorActive)}({nameof(PositionType)}.{nameof(PositionType.VestBack)}): {bHapticsManager.IsAnyMotorActive(PositionType.VestBack)}");
                    goto default;


                case ConsoleKey.I:
                    Console.WriteLine($"{nameof(bHapticsManager.GetDeviceStatus)}({nameof(PositionType)}.{nameof(PositionType.Vest)}): {bHapticsManager.GetDeviceStatus(PositionType.Vest).ToArrayString()}");
                    goto default;
                case ConsoleKey.O:
                    Console.WriteLine($"{nameof(bHapticsManager.GetDeviceStatus)}({nameof(PositionType)}.{nameof(PositionType.VestFront)}): {bHapticsManager.GetDeviceStatus(PositionType.VestFront).ToArrayString()}");
                    goto default;
                case ConsoleKey.P:
                    Console.WriteLine($"{nameof(bHapticsManager.GetDeviceStatus)}({nameof(PositionType)}.{nameof(PositionType.VestBack)}): {bHapticsManager.GetDeviceStatus(PositionType.VestBack).ToArrayString()}");
                    goto default;


                case ConsoleKey.NumPad1:
                    Console.WriteLine($"{nameof(bHapticsManager.IsDeviceConnected)}({nameof(PositionType)}.{nameof(PositionType.Vest)}): {bHapticsManager.IsDeviceConnected(PositionType.Vest)}");
                    goto default;


                case ConsoleKey.NumPad2:
                    testFeedback.Play();
                    goto default;


                case ConsoleKey.NumPad4:
                    Console.WriteLine($"{nameof(testFeedback)}.{nameof(testFeedback.IsRegistered)}(): {testFeedback.IsRegistered()}");
                    goto default;


                case ConsoleKey.NumPad7:
                    Console.WriteLine($"{nameof(testFeedback)}.{nameof(testFeedback.IsPlaying)}(): {testFeedback.IsPlaying()}");
                    goto default;


                case ConsoleKey.NumPad8:
                    bHapticsManager.Play("testPlayFront", 1000, PositionType.VestFront, new List<DotPoint> { new DotPoint { Index = 0, Intensity = 100 } });
                    goto default;
                case ConsoleKey.NumPad9:
                    bHapticsManager.Play("testPlayBack", 1000, PositionType.VestBack, new List<DotPoint> { new DotPoint { Index = 0, Intensity = 100 } });
                    goto default;

                default:
                    return false;
            }
        }
    }

    internal static class Extensions
    {
        internal static string ToArrayString<T>(this T[] arr) where T : IComparable<T>
        {
            int count = arr.Length;
            if (count <= 0)
                return "{ }";
            string returnval = "{";
            for (int i = 0; i < count; i++)
            {
                returnval += $" {arr[i]}";
                if (i < count - 1)
                    returnval += ",";
            }
            returnval += " }";
            return returnval;
        }
    }
}