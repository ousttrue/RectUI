﻿using System;
using System.Runtime.InteropServices;


namespace DesktopDll
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/desktop/controls/imagelistdrawflags
    /// </summary>
    public enum IDL : uint
    {
        NORMAL = 0x00000000,
    }

    public static class Comctl32
    {
        const string DLLNAME = "Comctl32.dll";

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/api/commctrl/nf-commctrl-imagelist_geticon
        /// </summary>
        /// <param name="himl"></param>
        /// <param name="i"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport(DLLNAME, CharSet = CharSet.Unicode)]
        public static extern HICON ImageList_GetIcon(
            IntPtr himl,
            int i,
            IDL flags
        );
    }
}
