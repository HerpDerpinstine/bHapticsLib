using System;
using System.Threading;
using bHapticsLib;

namespace TestApplication
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Initializing...");
            bHapticsManager.Debug = true;
            bHapticsManager.Initialize("bHapticsLib", "TestApplication");
            Console.WriteLine();

            Console.WriteLine($"Press 1 for {nameof(bHapticsManager.IsPlayerConnected)}()");
            Console.WriteLine();

            Console.WriteLine($"Press 2 for {nameof(bHapticsManager.GetConnectedDeviceCount)}()");
            Console.WriteLine($"Press 3 for {nameof(bHapticsManager.IsAnyDevicesConnected)}()");
            Console.WriteLine();

            Console.WriteLine($"Press 4 for {nameof(bHapticsManager.IsPlayingAny)}()");
            Console.WriteLine();

            Console.WriteLine($"Press NUMPAD-3 for {nameof(bHapticsManager.IsDeviceConnected)}({nameof(PositionType)}.{nameof(PositionType.Vest)})");
            Console.WriteLine();

            Console.WriteLine($"Press NUMPAD-4 for {nameof(bHapticsManager.IsFeedbackRegistered)}(\"testfeedback1\")");
            Console.WriteLine($"Press NUMPAD-5 for {nameof(bHapticsManager.IsFeedbackRegistered)}(\"testfeedback2\")");
            Console.WriteLine($"Press NUMPAD-6 for {nameof(bHapticsManager.IsFeedbackRegistered)}(\"testfeedback3\")");
            Console.WriteLine();

            Console.WriteLine($"Press NUMPAD-7 for {nameof(bHapticsManager.IsPlaying)}(\"testfeedback1\")");
            Console.WriteLine($"Press NUMPAD-8 for {nameof(bHapticsManager.IsPlaying)}(\"testfeedback2\")");
            Console.WriteLine($"Press NUMPAD-9 for {nameof(bHapticsManager.IsPlaying)}(\"testfeedback3\")");
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

            if (!bHapticsManager.Quit())
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

                case ConsoleKey.D1:
                    Console.WriteLine($"{nameof(bHapticsManager.IsPlayerConnected)}(): {bHapticsManager.IsPlayerConnected()}");
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


                case ConsoleKey.NumPad3:
                    Console.WriteLine($"{nameof(bHapticsManager.IsDeviceConnected)}({nameof(PositionType)}.{nameof(PositionType.Vest)}): {bHapticsManager.IsDeviceConnected(PositionType.Vest)}");
                    goto default;


                case ConsoleKey.NumPad4:
                    Console.WriteLine($"{nameof(bHapticsManager.IsFeedbackRegistered)}(\"testfeedback1\"): {bHapticsManager.IsFeedbackRegistered("testfeedback1")}");
                    goto default;
                case ConsoleKey.NumPad5:
                    Console.WriteLine($"{nameof(bHapticsManager.IsFeedbackRegistered)}(\"testfeedback2\"): {bHapticsManager.IsFeedbackRegistered("testfeedback2")}");
                    goto default;
                case ConsoleKey.NumPad6:
                    Console.WriteLine($"{nameof(bHapticsManager.IsFeedbackRegistered)}(\"testfeedback3\"): {bHapticsManager.IsFeedbackRegistered("testfeedback3")}");
                    goto default;


                case ConsoleKey.NumPad7:
                    Console.WriteLine($"{nameof(bHapticsManager.IsPlaying)}(\"testfeedback1\"): {bHapticsManager.IsPlaying("testfeedback1")}");
                    goto default;
                case ConsoleKey.NumPad8:
                    Console.WriteLine($"{nameof(bHapticsManager.IsPlaying)}(\"testfeedback2\"): {bHapticsManager.IsPlaying("testfeedback2")}");
                    goto default;
                case ConsoleKey.NumPad9:
                    Console.WriteLine($"{nameof(bHapticsManager.IsPlaying)}(\"testfeedback3\"): {bHapticsManager.IsPlaying("testfeedback3")}");
                    goto default;

                default:
                    return false;
            }
        }
    }
}