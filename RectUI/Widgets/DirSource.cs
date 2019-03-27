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

    public class DirSource : IListSource<FileSystemInfo>
    {
        List<FileSystemInfo> m_files = new List<FileSystemInfo>();

        public FileSystemInfo this[int index] => m_files[index];
        public int Count => m_files.Count;
        public event Action Updated;

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

                m_files.Clear();
                m_files.Add(m_current.Parent);
                try
                {
                    m_files.AddRange(
                        m_current.EnumerateFileSystemInfos()
                        );
                }
                catch (UnauthorizedAccessException)
                {
                    // do nothing
                }

                Updated?.Invoke();
            }
        }

        public DirSource(string path = ".")
        {
            Current = new DirectoryInfo(Path.GetFullPath(path));
        }

        public void ChangeDirectory(DirectoryInfo d)
        {
            Current = d;
        }

        public ListItemRegion<FileSystemInfo> CreateItem()
        {
            return new DirItemRegion(this);
        }

        public void LeftClicked(int index)
        {
            if(index<0 || index>= m_files.Count)
            {
                return;
            }
            var d = m_files[index] as DirectoryInfo;
            if (d != null)
            {
                ChangeDirectory(d);
            }
        }
    }

    public class DirItemRegion : ListItemRegion<FileSystemInfo>
    {
        public DirItemRegion(DirSource source) : base(source)
        {
        }

        public override IEnumerable<DrawCommand> GetIconCommands()
        {
            var f = Content as FileSystemInfo;
            var icon = SystemIcon.Get(f.FullName, true);
            return DrawCommandFactory.DrawImageListCommands(this,
                icon.ImageList, icon.ImageListIndex);
        }
    }
}
