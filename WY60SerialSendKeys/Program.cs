using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WY60SerialSendKeys
{
    internal class Program
    {
        // Get a handle to an application window.
        [System.Runtime.InteropServices.DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        // Activate an application window.
        [System.Runtime.InteropServices.DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private static List<IntPtr> SearchWindowTitles(string windowTitle)
        {
            Process[] processlist = Process.GetProcesses();
            List<IntPtr> foundProcessList = new List<IntPtr>();
            foreach (Process process in processlist)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle) && (process.MainWindowTitle.IndexOf(windowTitle) != -1))
                {
                    try
                    {
                        foundProcessList.Add(process.Handle);
                    }
                    catch
                    {
                        Console.WriteLine("Unable to obtain handle of Process: {0} ID: {1} Window title: {2}", process.ProcessName, process.Id, process.MainWindowTitle);
                        continue;
                    }
                    Console.WriteLine("[" + (foundProcessList.Count - 1) + "] Process: {0} ID: {1} Window title: {2}", process.ProcessName, process.Id, process.MainWindowTitle);
                }
            }
            return foundProcessList;
        }

        public class SerialKeyWriter
        {
            public SerialPort serialPort { get; set; }
            public IntPtr windowHandle { get; set; }
            private string inputString;
            public SerialKeyWriter(SerialPort _serialPort, IntPtr _windowHandle)
            {
                serialPort = _serialPort;
                windowHandle = _windowHandle;
            }

            public void PushKeys()
            {
                byte[] buffer = new byte[4];
                for (; ; )
                {
                    serialPort.Read(buffer, 0, 4);
                    inputString = System.Text.Encoding.Default.GetString(buffer);
                    SendKeys.SendWait(inputString);
                }
            }
        }

        private static void Main(string[] args)
        {
            SerialPort _serialPort;
            IntPtr windowHandle = IntPtr.Zero;
            List<IntPtr> foundProcessList = new List<IntPtr>();
            SerialKeyWriter keyWriter = null;
            Thread keyWriterThread = null;
            string userChoice;
            string userWindowClass;
            string userWindowTitle;
            int userChoiceIndex;

            Console.WriteLine("Note: will write to any foreground application, need to choose an application beforehand.");

            Console.WriteLine("Opening serial connection...");
            _serialPort = new SerialPort("COM8", 19200, Parity.None, 8, StopBits.One)
            {
                Handshake = Handshake.None,
                ReadTimeout = SerialPort.InfiniteTimeout,
                WriteTimeout = 500
            };
            _serialPort.Open();

            if (!_serialPort.IsOpen)
            {
                Console.WriteLine("Could not open serial port!");
                return;
            }
            Console.WriteLine("Serial connection established.");


            Console.WriteLine("Finding VSCode window...");
            foundProcessList = SearchWindowTitles("Visual Studio Code")


            for (; ; )
            {
                Console.Write("Choose a process or enter a command ('q' to quit, 'none' to enter window class and title, or 'search to search for a window'): ");
                if ((userChoice = Console.ReadLine()) == "q")
                {
                    return;
                }
                else if (userChoice == "none")
                {
                    Console.Write("Window Class: ");
                    userWindowClass = Console.ReadLine();
                    Console.Write("Window Title: ");
                    userWindowTitle = Console.ReadLine();
                    if ((windowHandle = FindWindow(userWindowClass, userWindowTitle)) == IntPtr.Zero)
                    {
                        Console.WriteLine("Unable to find window!");
                    }
                    continue;
                }
                else if (userChoice == "search")
                {
                    Console.Write("Window Title: ");
                    userWindowTitle = Console.ReadLine();
                    Console.WriteLine("Results: ");
                    foundProcessList = SearchWindowTitles(userWindowTitle);
                    continue;
                }
                else
                {
                    try
                    {
                        userChoiceIndex = int.Parse(userChoice);
                    }
                    catch
                    {
                        Console.WriteLine("Invalid choice!");
                        continue;
                    }
                    if (userChoiceIndex >= foundProcessList.Count || userChoiceIndex < 0)
                    {
                        Console.WriteLine("Invalid choice!");
                        continue;
                    }
                    windowHandle = foundProcessList[userChoiceIndex];
                }
                if (windowHandle == IntPtr.Zero)
                {
                    Console.WriteLine("Please choose a window.");
                    continue;
                }
                SetForegroundWindow(windowHandle);
                if (keyWriter == null)
                {
                    keyWriter = new SerialKeyWriter(_serialPort, windowHandle);
                }

                keyWriter.windowHandle = windowHandle;
                if (keyWriterThread == null)
                {
                    keyWriterThread = new Thread(new ThreadStart(keyWriter.PushKeys));
                }
                if (!keyWriterThread.IsAlive)
                {
                    keyWriterThread.Start();
                }

                Console.WriteLine("Writing data... listening for prompts.");
            }
        }
    }
}
