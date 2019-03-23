using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RectUI
{
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

        public Func<UIContext, int, ContentRegion<T>, IEnumerable<Graphics.DrawCommand>> ItemGetDrawCommands;

        IListSource<T> m_source;

        List<ContentRegion<T>> m_regions = new List<ContentRegion<T>>();
        public ListRegion(IListSource<T> source)
        {
            m_source = source;

            source.Updated += () =>
            {
                Layout();
            };
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

        IEnumerable<RectRegion> Layout()
        {
            var count = Rect.Height / ItemHeight + 1;

            var index = m_scrollY / ItemHeight;

            var y = Rect.Y + (index * ItemHeight - m_scrollY);
            for (int i = 0; i < count; ++i, ++index)
            {
                ContentRegion<T> r = null;
                if (i < m_regions.Count)
                {
                    r = m_regions[i];
                }
                else
                {
                    r = new ContentRegion<T>
                    {
                        Parent = this,
                        OnGetDrawCommands = (uiContext, rr) =>
                        {
                            return ItemGetDrawCommands(uiContext, i, rr as ContentRegion<T>);
                        }
                    };
                    r.LeftClicked += x => R_LeftClicked(x as ContentRegion<T>);
                    m_regions.Add(r);
                }

                r.Rect = new Rect(Rect.X, y, Rect.Width, ItemHeight);
                if (index >= m_source.Count)
                {
                    break;
                }

                if (index < 0)
                {

                }
                else
                {
                    r.Content = m_source[index];

                    yield return r;
                }

                y += ItemHeight;
            }
        }

        public event Action<int, T> ItemLeftClicked;
        private void R_LeftClicked(ContentRegion<T> r)
        {
            var index = m_regions.IndexOf(r);
            var first = ScrollY / ItemHeight;
            ItemLeftClicked?.Invoke(first+index, r.Content);
        }

        public override IEnumerable<RectRegion> Traverse()
        {
            foreach (var r in Layout())
            {
                foreach (var x in r.Traverse())
                {
                    yield return x;
                }
            }
        }

        public override RectRegion MouseMove(int x, int y)
        {
            foreach (var r in Layout())
            {
                var hover = r.MouseMove(x, y);
                if (hover != null)
                {
                    return hover;
                }
            }

            if(Rect.Contains(x, y))
            {
                return this;
            }

            return null;
        }
    }
}
