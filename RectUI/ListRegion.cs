using DesktopDll;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace RectUI
{
    class SystemIcon
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

    public interface IListSource<T>
    {
        int Count { get; }
        T this[int index] { get; }
        event Action Updated;
    }

    public class ListSource<T> : IListSource<T>, IEnumerable<T>
    {
        List<T> m_items = new List<T>();
        public int Count => m_items.Count;

        public T this[int index] { get { return m_items[index]; } }

        public event Action Updated;

        public IEnumerator<T> GetEnumerator()
        {
            return m_items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T value)
        {
            m_items.Add(value);
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
                //m_files.Add(m_current);
                try
                {
                    m_files.AddRange(
                        m_current.EnumerateFileSystemInfos()
                        );
                }
                catch(UnauthorizedAccessException)
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
    }

    public class ListRegion<T> : RectRegion
    {
        public override Rect Rect
        {
            get => base.Rect;
            set
            {
                base.Rect = value;
                Layout();
            }
        }

        int m_itemHeight = 18;
        public int ItemHeight
        {
            get { return m_itemHeight = 18; }
            set
            {
                if (m_itemHeight == value) return;
                m_itemHeight = value;
                if (m_itemHeight < 1)
                {
                    m_itemHeight = 1;
                }
            }
        }

        public Func<UIContext, int, RectRegion, IEnumerable<Graphics.DrawCommand>> ItemGetDrawCommands;

        IListSource<T> m_source;

        public ListRegion(IListSource<T> source)
        {
            m_source = source;

            source.Updated += () =>
            {
                Layout();
            };

            ItemGetDrawCommands = OnItemGetDrawCommands;
        }

        IEnumerable<Graphics.DrawCommand> OnItemGetDrawCommands(UIContext uiContext, int i, RectRegion r)
        {
            if (r.Content == null)
            {
                return Enumerable.Empty<Graphics.DrawCommand>();
            }

            var rect = r.Rect.ToSharpDX();
            rect.X += 16;
            rect.Width -= 16;
            var commands = DrawCommandFactory.DrawRectCommands(rect,
                r.GetFillColor(uiContext),
                r.GetBorderColor(uiContext));

            var label = r.Content.ToString();
            /*
            if (dir.Current.Parent.FullName == r.Content.FullName)
            {
                label = "..";
            }
            */

            /*
            var icon = SystemIcon.Get(r.Content.FullName, true);
            commands = commands.Concat(DrawCommandFactory.DrawImageListCommands(uiContext, r,
                icon.ImageList, icon.ImageListIndex));
            */

            var text = DrawCommandFactory.DrawTextCommands(r,
                GetTextColor(uiContext), "MS Gothic", ItemHeight,
                21, 3, 5, 2,
                label);
            commands = commands.Concat(text);

            return commands;
        }

        int m_scrollY = 0;
        public int ScrollY
        {
            get { return m_scrollY; }
            set
            {
                if (m_scrollY == value) return;
                m_scrollY = value;
                Layout();
            }
        }

        void Layout()
        {
            var count = Rect.Height / ItemHeight + 1;

            var index = m_scrollY / ItemHeight;

            var y = Rect.Y + (index * ItemHeight - m_scrollY);
            int i = 0;
            for (; i < count; ++i, ++index)
            {
                RectRegion r = null;
                if (i < Children.Count)
                {
                    r = Children[i];
                }
                else
                {
                    r = new RectRegion
                    {
                        Parent = this,
                        OnGetDrawCommands = (uiContext, rr) =>
                        {
                            return ItemGetDrawCommands(uiContext, i, rr);
                        }
                    };
                    r.LeftClicked += x => R_LeftClicked(x);
                    Children.Add(r);
                }

                r.Rect = new Rect(Rect.X, y, Rect.Width, ItemHeight);
                if (index < 0 || index >= m_source.Count)
                {
                    r.Content = null;
                }
                else
                {
                    r.Content = m_source[index];
                }

                y += ItemHeight;
            }
        }

        public event Action<int, object> ItemLeftClicked;
        private void R_LeftClicked(RectRegion r)
        {
            var index = Children.IndexOf(r);
            var first = ScrollY / ItemHeight;
            ItemLeftClicked?.Invoke(first+index, r.Content);
        }
    }
}
