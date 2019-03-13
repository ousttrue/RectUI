using System;
using System.Reflection;
using System.Runtime.InteropServices;


namespace DesktopDll
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/desktop/inputdev/mouse-input-notifications
    /// </summary>
    public enum WM : uint
    {
        DESTROY = 0x0002,
        RESIZE = 0x0005,
        PAINT = 0x000F,
        LBUTTONUP = 0x0202,
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/desktop/learnwin32/creating-a-window
    /// </summary>
    public class Window
    {
        const string CLASS_NAME = "class";
        const string WINDOW_NAME = "window";

        HWND _hwnd;
        public IntPtr WindowHandle
        {
            get { return _hwnd.Value; }
        }

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
                    User32.PostQuitMessage(0);
                    return 0;

                case WM.RESIZE:
                    OnResize?.Invoke(lParam.LowWord, lParam.HiWord);
                    return 0;

                case WM.PAINT:
                    OnPaint?.Invoke();
                    return 0;
            }
            return User32.DefWindowProcW(hwnd, msg, wParam, lParam);
        }

        public event Action OnPaint;
        public event Action<int, int> OnResize;

        public void Show()
        {
            User32.ShowWindow(_hwnd, SW.SHOW);
        }

        MSG _msg;
        public bool MessageLoop()
        {
            if (!User32.GetMessageW(ref _msg, 0, 0, 0))
            {
                return false;
            };
            User32.TranslateMessage(ref _msg);
            User32.DispatchMessage(ref _msg);
            return true;
        }
    }
}
