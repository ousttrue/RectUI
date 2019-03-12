using System;
using System.Runtime.InteropServices;


namespace DesktopDll
{
    #region data types
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/desktop/WinProg/windows-data-types
    /// </summary>

    [StructLayout(LayoutKind.Sequential)]
    public struct ATOM
    {
        public uint Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BOOL
    {
        public int Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DWORD
    {
        public uint Value;
        public static implicit operator DWORD(uint value)
        {
            return new DWORD { Value = value };
        }
        public static implicit operator DWORD(int value)
        {
            return new DWORD { Value = (uint)value };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HBRUSH
    {
        public IntPtr Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HCURSOR
    {
        public IntPtr Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HICON
    {
        public IntPtr Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HINSTANCE
    {
        public IntPtr Value;
        public static implicit operator HINSTANCE(IntPtr value)
        {
            return new HINSTANCE { Value = value };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HMENU
    {
        public IntPtr Value;
        public static implicit operator HMENU(IntPtr value)
        {
            return new HMENU { Value = value };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HWND
    {
        public IntPtr Value;
        public static implicit operator HWND(IntPtr value)
        {
            return new HWND { Value = value };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LONG
    {
        public int Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LPARAM
    {
        public IntPtr Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LPVOID
    {
        public IntPtr Value;
        public static implicit operator LPVOID(IntPtr value)
        {
            return new LPVOID { Value = value };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LRESULT
    {
        public IntPtr Value;
        public static implicit operator LRESULT(IntPtr value)
        {
            return new LRESULT { Value = value };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UINT
    {
        public uint Value;

        public static implicit operator UINT(uint value)
        {
            return new UINT { Value = value };
        }
        public static implicit operator UINT(int value)
        {
            return new UINT { Value = (uint)value };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WPARAM
    {
        public IntPtr Value;
    }
    #endregion

    /// <summary>
    /// https://msdn.microsoft.com/en-us/library/ms633573(v=VS.85).aspx
    /// </summary>
    /// <returns></returns>
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate LRESULT WNDPROC(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

    #region Structs
    [Flags]
    public enum CS : uint
    {
        VREDRAW = 0x0001,
        HREDRAW = 0x0002,
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-tagwndclassexw
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WNDCLASSEXW
    {
        public UINT cbSize;
        public CS style;
        public WNDPROC lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public HINSTANCE hInstance;
        public HICON hIcon;
        public HCURSOR hCursor;
        public HBRUSH hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public HICON hIconSm;
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/previous-versions//dd162805(v=vs.85)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct POINT
    {
        public LONG x;
        public LONG y;
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-tagmsg
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct MSG
    {
        public HWND hwnd;
        public WM message;
        public WPARAM wParam;
        public LPARAM lParam;
        public DWORD time;
        public POINT pt;
        public DWORD lPrivate;
    }
    #endregion
}
