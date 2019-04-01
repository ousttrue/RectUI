using DesktopDll;
using RectUI.Graphics;
using SharpDX;
using System;
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
                    RaiseUpdate();
                }
            };
        }

        public void ChangeDirectory(DirectoryInfo d)
        {
            Current = d;
        }

        public override ListItemRegion<FileSystemInfo> CreateItem()
        {
            return new DirItemRegion();
        }
    }

    public class DirItemRegion : ListItemRegion<FileSystemInfo>
    {
        protected override void GetIconCommands(IDrawProcessor rpc, bool isActive, bool isHover)
        {
            //var icon = SystemIcon.Get(Content.FullName, true);
            rpc.FileIcon(ID, Rect.ToSharpDX(), Content.FullName);
        }

        protected override void GetTextCommands(IDrawProcessor rpc, bool isActive, bool isHover)
        {
            var color = GetTextColor(isActive, isHover);

            // todo: m_padding
            var rect = new RectangleF(
                m_padding.Left + Rect.X,
                m_padding.Top + Rect.Y,
                Rect.Width - m_padding.Horizontal,
                Rect.Height - m_padding.Vertical
                );
            rpc.Text(ID, rect,
                color,
                new FontInfo
                {
                    Font = "MS Gothic",
                    Size = Rect.Height - m_padding.Top - m_padding.Bottom,
                },
                new TextInfo
                {
                    Text = Content.Name,
                    HorizontalAlignment = TextHorizontalAlignment.Left,
                    VerticalAlignment = TextVerticalAlignment.Center,
                }
            );
        }
    }
}
