using RectUI.Graphics;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;


namespace RectUI.Widgets
{
    public class DirSource : IListSource<FileSystemInfo>
    {
        DirectoryInfo m_current;

        public event Action Updated;
        void RaiseUpdated()
        {
            Updated?.Invoke();
        }

        List<FileSystemInfo> m_items = new List<FileSystemInfo>();

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

                RaiseUpdated();
            }
        }
        public int Count => m_items.Count;
        public FileSystemInfo this[int index] => m_items[index];

        public void GoUp()
        {
            if(Current!=null && Current.Parent != null)
            {
                Current = Current.Parent;
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
        }

        public ListItemRegion<FileSystemInfo> CreateItem()
        {
            return new DirItemRegion();
        }

        public void Enter(int index)
        {
            var f = m_items[index];
            var d = f as DirectoryInfo;
            if (d != null)
            {
                Current = d;
            }
        }
    }

    public class DirItemRegion : ListItemRegion<FileSystemInfo>
    {
        protected override void GetIconCommands(IDrawProcessor rpc, bool isActive, bool isHover)
        {
            if (!SourceIndex.HasValue) return;

            //var icon = SystemIcon.Get(Content.FullName, true);
            rpc.FileIcon(ID, Rect.ToSharpDX(), Content.FullName);
        }

        protected override void GetTextCommands(IDrawProcessor rpc, bool isActive, bool isHover)
        {
            if (!SourceIndex.HasValue) return;

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
