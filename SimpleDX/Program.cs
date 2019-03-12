using System;
using System.Reflection;
using System.Runtime.InteropServices;
using DesktopDll;


namespace SimpleDX
{
    class Program
    {
        const string CLASS_NAME = "class";

        static LRESULT WndProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
        {
            return IntPtr.Zero;
        }

        static void Main(string[] args)
        {
            var ms = Assembly.GetEntryAssembly().GetModules();
            var hInstance = Marshal.GetHINSTANCE(ms[0]);

            var wc = new WNDCLASSEXW
            {
                cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEXW)),
                style = CS.VREDRAW | CS.HREDRAW,
                lpszClassName = CLASS_NAME,
                lpfnWndProc = WndProc,
                hInstance = hInstance,
            };
            var register = User32.RegisterClassExW(ref wc);
            if (register == 0)
            {
                throw new Exception();
            }

            var hwnd = User32.CreateWindowExW(0, CLASS_NAME, "windiow", WS.OVERLAPPEDWINDOW,
                0, 0, 640, 480,
                IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
            if (hwnd == IntPtr.Zero)
            {
                throw new Exception();
            }

            Console.WriteLine(hwnd);
        }
    }
}
