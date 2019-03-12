using DesktopDll;
using System;
using System.Reflection;
using System.Runtime.InteropServices;


namespace SimpleDX
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/desktop/learnwin32/creating-a-window
    /// </summary>
    class Window
    {
        const string CLASS_NAME = "class";
        const string WINDOW_NAME = "window";

        HWND _hwnd;

        public static Window Create()
        {
            var ms = Assembly.GetEntryAssembly().GetModules();
            var hInstance = Marshal.GetHINSTANCE(ms[0]);

            var window = new Window();

            var wc = new WNDCLASSEXW
            {
                cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEXW)),
                style = CS.VREDRAW | CS.HREDRAW,
                lpszClassName = CLASS_NAME,
                lpfnWndProc = window.WndProc,
                hInstance = hInstance,
            };
            var register = User32.RegisterClassExW(ref wc);
            if (register == 0)
            {
                return null;
            }

            var hwnd = User32.CreateWindowExW(0, CLASS_NAME, WINDOW_NAME, WS.OVERLAPPEDWINDOW,
                User32.CW_USEDEFAULT,
                User32.CW_USEDEFAULT,
                User32.CW_USEDEFAULT,
                User32.CW_USEDEFAULT,
                IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
            if (hwnd == IntPtr.Zero)
            {
                return null;
            }

            window._hwnd = hwnd;
            return window;
        }

        LRESULT WndProc(HWND hwnd, WM msg, WPARAM wParam, LPARAM lParam)
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

        public void Show()
        {
            User32.ShowWindow(_hwnd, SW.SHOW);
        }

        public void MessageLoop()
        {
            var msg = default(MSG);

            while (true)
            {
                if (!User32.GetMessageW(ref msg, 0, 0, 0))
                {
                    break;
                };
                User32.TranslateMessage(ref msg);
                User32.DispatchMessage(ref msg);
            }
        }
    }
}
