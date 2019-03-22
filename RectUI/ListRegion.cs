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

    public class DirSource : IListSource<string>
    {
        List<string> m_files = new List<string>();

        public string this[int index] => m_files[index];
        public int Count => m_files.Count;
        public event Action Updated;

        string m_current;
        public string Current
        {
            get { return m_current; }
            set
            {
                if (m_current == value)
                {
                    return;
                }
                m_current = Path.GetFullPath(value);

                m_files.Clear();
                m_files.AddRange(
                    Directory.GetFileSystemEntries(m_current)
                    .Select(x => Path.GetFileName(x))
                    );

                Updated?.Invoke();
            }
        }

        public DirSource(string path = ".")
        {
            Current = Path.GetFullPath(path);
        }
    }

    public class ListRegion : RectRegion
    {
        int m_height = 18;

        IListSource<string> m_source;

        List<RectRegion> m_regions = new List<RectRegion>();
        List<TextLabelDrawer> m_drawers = new List<TextLabelDrawer>();
        public ListRegion(IListSource<string> source)
        {
            m_source = source;

            source.Updated += () =>
            {
                Layout();
            };
        }

        IEnumerable<RectRegion> Layout()
        {
            var count = Math.Min(m_source.Count, Rect.Height / m_height + 1);
            var y = Rect.Y;
            for (int i = 0; i < count; ++i)
            {
                RectRegion r = null;
                if (i < m_regions.Count)
                {
                    r = m_regions[i];
                }
                else
                {
                    r = new RectRegion
                    {
                    };
                    r.LeftClicked += () => R_LeftClicked(r);
                    m_regions.Add(r);
                }

                r.Rect = new Rect(Rect.X, y, Rect.Width, m_height);
                r.Drawer = new TextLabelDrawer
                {
                    Label = m_source[i]
                };

                yield return r;

                y += m_height;
            }
        }

        public event Action<int, string> LeftClicked;
        private void R_LeftClicked(RectRegion r)
        {
            var index = m_regions.IndexOf(r);
            LeftClicked?.Invoke(index, (r.Drawer as TextLabelDrawer).Label);
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

        public override RectRegion MouseMove(int parentX, int parentY)
        {
            var x = parentX - Rect.X;
            var y = parentY - Rect.Y;

            foreach (var r in Layout())
            {
                var hover = r.MouseMove(x, y);
                if (hover != null)
                {
                    return hover;
                }
            }

            return null;
        }
    }
}
