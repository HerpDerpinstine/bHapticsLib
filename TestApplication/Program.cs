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
            Console.WriteLine("Waiting for bHaptics Player...");
            while (!bHapticsManager.IsPlayerConnected())
                Thread.Sleep(1);
            Console.WriteLine("Connected to bHaptics Player!");
            Console.WriteLine();

            // Print Keybinds
            Console.WriteLine("Press Enter to Disconnect.");
            Console.WriteLine();

            while (Console.ReadKey(false).Key != ConsoleKey.Enter)
            {
                KeyCheck();
                Thread.Sleep(1);
            }

            Console.WriteLine();
            Console.WriteLine("Disconnecting from bHaptics Player...");
            bHapticsManager.Quit();

            Console.WriteLine();
            Console.WriteLine("Disconnected from bHaptics Player!");
            Console.WriteLine("Press Enter to Exit.");
            while (Console.ReadKey(false).Key != ConsoleKey.Enter)
                Thread.Sleep(1);
        }

        static void KeyCheck()
        {

        }
    }
}