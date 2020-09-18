using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace WY60SerialSendKeys
{

    class Program
    {
        // Get a handle to an application window.
        [System.Runtime.InteropServices.DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        // Activate an application window.
        [System.Runtime.InteropServices.DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        static void Main(string[] args)
        {
            SerialPort _serialPort;
            IntPtr vscodeHandle;
            string windowTitle;
            _serialPort = new SerialPort("COM8", 19200, Parity.None, 8, StopBits.One);
            _serialPort.Handshake = Handshake.None;
            Console.WriteLine("Reading terminal settings...");
            _serialPort.ReadTimeout = SerialPort.InfiniteTimeout;
            _serialPort.WriteTimeout = 500;
            _serialPort.Open();
            if (!_serialPort.IsOpen)
            {
                Console.WriteLine("Could not open serial port!");
                return;
            }
            vscodeHandle = FindWindow("Chrome_WidgetWin_1", "Welcome - Visual Studio Code [Unsupported]");
            while (vscodeHandle == IntPtr.Zero)
            {
                MessageBox.Show("VSCode not running or window title unknown, specify here or enter q to quit: ");
                if ((windowTitle = Console.ReadLine()) == "q")
                    return;
                vscodeHandle = FindWindow("Chrome_WidgetWin_1", windowTitle);
            }
            SetForegroundWindow(vscodeHandle);
            for (; ; )
            {
                string inputstring = _serialPort.ReadExisting();
                Console.WriteLine("Read: " + inputstring);
                SendKeys.SendWait(inputstring);
            }
        }
    }
}
