using DesktopDll;
using System;
using System.Runtime.InteropServices;

namespace RectUI.Application
{
    public class SystemIcon
    {
        public IntPtr ImageList
        {
            get;
            private set;
        }

        public int ImageListIndex
        {
            get;
            private set;
        }

        public static SystemIcon Get(string path, bool isSmall)
        {
            var flags = SHGFI.SYSICONINDEX;
            if (isSmall)
            {
                flags |= SHGFI.SMALLICON;
            }
            var sfi = default(SHFILEINFOW);
            var result = Shell32.SHGetFileInfoW(path, -1,
                ref sfi, Marshal.SizeOf<SHFILEINFOW>(),
                flags);
            if (result == IntPtr.Zero)
            {
                return null;
            }

            return new SystemIcon
            {
                ImageList = result,
                ImageListIndex = sfi.iIcon,
            };
        }
    }
}
