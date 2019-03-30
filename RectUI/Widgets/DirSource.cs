using DesktopDll;
using RectUI.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;


namespace RectUI.Widgets
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

    public class DirSource : ListSource<FileSystemInfo>
    {
        DirectoryInfo m_current;
        public DirectoryInfo Current
        {
            get { return m_current; }
            set
            {
                if (m_current == value)
                {
                    return;
                }
                m_current = value;

                m_items.Clear();
                m_items.Add(m_current.Parent);
                try
                {
                    m_items.AddRange(
                        m_current.EnumerateFileSystemInfos()
                        );
                }
                catch (UnauthorizedAccessException)
                {
                    // do nothing
                }

                RaiseUpdate();
            }
        }

        public DirSource() : this(".")
        { }

        public DirSource(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = ".";
            }
            Current = new DirectoryInfo(Path.GetFullPath(path));

            Entered += f =>
            {
                var d = f as DirectoryInfo;
                if (d != null)
                {
                    ChangeDirectory(d);
                }
            };
        }

        public void ChangeDirectory(DirectoryInfo d)
        {
            Current = d;
        }

        public override ListItemRegion<FileSystemInfo> CreateItem()
        {
            return new DirItemRegion(this);
        }
    }

    public class DirItemRegion : ListItemRegion<FileSystemInfo>
    {
        public DirItemRegion(DirSource source) : base(source)
        {
        }

        public override IEnumerable<D2DDrawCommand> GetIconCommands()
        {
            var f = Content as FileSystemInfo;
            var icon = SystemIcon.Get(f.FullName, true);

            yield return new D2DDrawCommand
            {
                RegionID = ID,
                Rectangle = Rect.ToSharpDX(),
                DrawType = DrawType.ImageList,
                Icon = icon.ImageList,
                ImageListIndex = icon.ImageListIndex,
            };
        }
    }
}
