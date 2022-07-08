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

            Console.WriteLine($"Press F1 for {nameof(bHapticsManager.IsPlayerConnected)}()");
            Console.WriteLine($"Press F2 for {nameof(bHapticsManager.IsAnyDevicesConnected)}()");
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

            if (bHapticsManager.IsPlayerConnected())
            {
                Console.WriteLine();
                Console.WriteLine("Disconnecting from bHaptics Player...");
                Console.WriteLine();
                if (!bHapticsManager.Quit())
                    Console.WriteLine("Failed to Disconnect!");
                else
                    Console.WriteLine("Disconnected from bHaptics Player!");
            }

            Console.WriteLine("Press Enter to Exit.");
            while (Console.ReadKey(false).Key != ConsoleKey.Enter)
                Thread.Sleep(1);
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

                case ConsoleKey.F1:
                    Console.WriteLine($"{nameof(bHapticsManager.IsPlayerConnected)}(): {bHapticsManager.IsPlayerConnected()}");
                    goto default;
                case ConsoleKey.F2:
                    Console.WriteLine($"{nameof(bHapticsManager.IsAnyDevicesConnected)}(): {bHapticsManager.IsAnyDevicesConnected()}");
                    goto default;

                default:
                    return false;
            }
        }
    }
}