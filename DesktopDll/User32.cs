using System;
using System.Runtime.InteropServices;


namespace DesktopDll
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/desktop/winmsg/window-styles
    /// </summary>
    [Flags]
    public enum WS : uint
    {
        OVERLAPPED = 0x00000000,
        MAXIMIZEBOX = 0x00010000,
        MINIMIZEBOX = 0x00020000,
        THICKFRAME = 0x00040000,
        SYSMENU = 0x00080000,
        CAPTION = 0x00C00000,
        OVERLAPPEDWINDOW = (OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX)
    }

    public static class User32
    {
        const string DLLNAME = "User32.dll";

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-registerclassexw
        /// </summary>
        /// <param name="Arg1"></param>
        /// <returns></returns>
        [DllImport(DLLNAME)]
        public static extern uint RegisterClassExW(ref WNDCLASSEXW Arg1);

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-createwindowexw
        /// </summary>
        /// <param name="dwExStyle"></param>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <param name="dwStyle"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="hWndParent"></param>
        /// <param name="hMenu"></param>
        /// <param name="hInstance"></param>
        /// <param name="lpParam"></param>
        /// <returns></returns>
        [DllImport(DLLNAME, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateWindowExW(
            DWORD dwExStyle,
            string lpClassName,
            string lpWindowName,
            WS dwStyle,
            int X,
            int Y,
            int nWidth,
            int nHeight,
            HWND hWndParent,
            HMENU hMenu,
            HINSTANCE hInstance,
            LPVOID lpParam
        );
    }
}
