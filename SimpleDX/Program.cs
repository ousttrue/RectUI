using System;
using System.Reflection;
using System.Runtime.InteropServices;
using DesktopDll;


namespace SimpleDX
{
    class Program
    {
        const string CLASS_NAME = "class";
        const string WINDOW_NAME = "window";

        static LRESULT WndProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
        {
            return IntPtr.Zero;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/learnwin32/creating-a-window
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        static void Main(string[] args)
        {
            var ms = Assembly.GetEntryAssembly().GetModules();
            var hInstance = Marshal.GetHINSTANCE(ms[0]);

            var wc = new WNDCLASSEXW
            {
                cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEXW)),
                style = CS.VREDRAW | CS.HREDRAW,
                lpszClassName = CLASS_NAME,
                lpfnWndProc = User32.DefWindowProcW,
                hInstance = hInstance,
            };
            var register = User32.RegisterClassExW(ref wc);
            if (register == 0)
            {
                throw new Exception("RegisterClassExW");
            }

            var hwnd = User32.CreateWindowExW(0, CLASS_NAME, WINDOW_NAME, WS.OVERLAPPEDWINDOW,
                0, 0, 640, 480,
                IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
            if (hwnd == IntPtr.Zero)
            {
                throw new Exception("CreateWindowExW");
            }

            User32.ShowWindow(hwnd, SW.SHOW);

            var msg = default(MSG);
            while (true)
            {
                User32.GetMessage(ref msg, hwnd, 0, 0);
                if (msg.message == WM.LBUTTONUP) break;
                User32.DispatchMessage(ref msg);
            }
        }
    }
}
