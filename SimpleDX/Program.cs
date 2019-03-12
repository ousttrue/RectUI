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

        static LRESULT WndProc(HWND hwnd, WM msg, WPARAM wParam, LPARAM lParam)
        {
            switch (msg)
            {
                case WM.DESTROY:
                    //User32.MessageBoxW(hwnd, "メッセージ", "caption", MB.ICONINFORMATION);
                    User32.PostQuitMessage(0);
                    return 0;
            }
            return User32.DefWindowProcW(hwnd, msg, wParam, lParam);
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
                lpfnWndProc = WndProc,
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
                if (!User32.GetMessageW(ref msg, 0, 0, 0))
                {
                    break;
                };
                User32.DispatchMessage(ref msg);
            }
        }
    }
}
