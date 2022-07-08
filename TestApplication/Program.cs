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
            bHapticsManager.Initialize("bHapticsLib", "TestApplication");
            Console.WriteLine();

            Console.WriteLine($"Press 1 for {nameof(bHapticsManager.IsPlayerConnected)}()");
            Console.WriteLine();

            Console.WriteLine($"Press 2 for {nameof(bHapticsManager.GetConnectedDeviceCount)}()");
            Console.WriteLine($"Press 3 for {nameof(bHapticsManager.IsAnyDevicesConnected)}()");
            Console.WriteLine();

            Console.WriteLine($"Press NUMPAD-3 for {nameof(bHapticsManager.IsDeviceConnected)}({nameof(PositionType)}.{nameof(PositionType.Vest)})");
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

                case ConsoleKey.NumPad3:
                    Console.WriteLine($"{nameof(bHapticsManager.IsDeviceConnected)}({nameof(PositionType)}.{nameof(PositionType.Vest)}): {bHapticsManager.IsDeviceConnected(PositionType.Vest)}");
                    goto default;

                default:
                    return false;
            }
        }
    }
}