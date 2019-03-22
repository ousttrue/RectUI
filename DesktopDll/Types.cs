using System;
using System.Runtime.InteropServices;


namespace DesktopDll
{
    #region data types
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/desktop/WinProg/windows-data-types
    /// </summary>

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct ATOM
    {
        public uint Value;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct BOOL
    {
        public int Value;
        public static implicit operator BOOL(int value)
        {
            return new BOOL { Value = value };
        }
        public static implicit operator BOOL(bool value)
        {
            return new BOOL { Value = value ? 1: 0 };
        }
        public static implicit operator bool(BOOL value)
        {
            return value.Value != 0;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
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


    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct HANDLE
    {
        public IntPtr Value;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct HBRUSH
    {
        public IntPtr Value;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct HCURSOR
    {
        public IntPtr Value;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct HDC
    {
        public IntPtr Value;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct HICON
    {
        public IntPtr Value;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct HINSTANCE
    {
        public IntPtr Value;
        public static implicit operator HINSTANCE(IntPtr value)
        {
            return new HINSTANCE { Value = value };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct HMENU
    {
        public IntPtr Value;
        public static implicit operator HMENU(IntPtr value)
        {
            return new HMENU { Value = value };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct HWND
    {
        public IntPtr Value;
        public static implicit operator HWND(IntPtr value)
        {
            return new HWND { Value = value };
        }
        public static implicit operator HWND(int value)
        {
            return new HWND { Value = new IntPtr(value) };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct LONG
    {
        public int Value;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct LPARAM
    {
        public IntPtr Value;

        public int LowWord
        {
            get
            {
                return (int)(Value.ToInt64() & short.MaxValue);
            }
        }

        public int HiWord
        {
            get
            {
                return (int)((Value.ToInt64() >> 16) & short.MaxValue);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct LPVOID
    {
        public IntPtr Value;
        public static implicit operator LPVOID(IntPtr value)
        {
            return new LPVOID { Value = value };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct LRESULT
    {
        public IntPtr Value;
        public static implicit operator LRESULT(int value)
        {
            return new LRESULT { Value = new IntPtr(value) };
        }
        public static implicit operator LRESULT(IntPtr value)
        {
            return new LRESULT { Value = value };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
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

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
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
    public delegate LRESULT WNDPROC(HWND hwnd, WM uMsg, WPARAM wParam, LPARAM lParam);

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
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct WNDCLASSEXW
    {
        public UINT cbSize;
        public CS style;
        public IntPtr lpfnWndProc;
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
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct POINT
    {
        public LONG x;
        public LONG y;
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/previous-versions//dd162897(v=vs.85)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct RECT
    {
        public LONG left;
        public LONG top;
        public LONG right;
        public LONG bottom;
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-tagmsg
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
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

    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-tagpaintstruct
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct PAINTSTRUCT
    {
        HDC hdc;
        BOOL fErase;
        RECT rcPaint;
        BOOL fRestore;
        BOOL fIncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        Byte[] rgbReserved;
    }
    #endregion
}
