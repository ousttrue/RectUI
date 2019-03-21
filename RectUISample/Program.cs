using DesktopDll;
using RectUI;
using System;
using System.Collections;
using System.Collections.Generic;


namespace RectUISample
{
    public interface IListSource<T>
    {
        int Count { get; }
        T this[int index] { get; }
    }

    public class ListSource<T>: IListSource<T>, IEnumerable<T>
    {
        List<T> m_items = new List<T>();
        public int Count => m_items.Count;

        public T this[int index]
        {
            get
            {
                return m_items[index];
            }
        }

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

    public class ListRegion: RectRegion
    {
        int m_height = 18;

        IListSource<string> m_source;

        List<RectRegion> m_regions = new List<RectRegion>();
        List<TextLabelDrawer> m_drawers = new List<TextLabelDrawer>();
        public ListRegion(IListSource<string> source)
        {
            m_source = source;
        }

        IEnumerable<RectRegion> Layout()
        {
            var count = Math.Min(m_source.Count, Rect.Height / m_height);
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

        public override IEnumerable<RectRegion> Traverse()
        {
            foreach(var r in Layout())
            {
                foreach(var x in r.Traverse())
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


    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine($"IntPtr.Size = {IntPtr.Size} bytes");

            // create window
            var window = Window.Create();
            window.Show();

            // build UI
#if false
            var root = new HorizontalSplitter
            {
                Rect = new Rect(window.Width, window.Height)
            };
            root.Add(new RectRegion
            {
                Drawer = new RectDrawer(),
            });
            root.Add(new RectRegion
            {
                Drawer = new RectDrawer(),
            });
#else
            var root = new ListRegion(new ListSource<string>
            {
                "a", "b", "c",
            })
            {
                Rect = new Rect(window.Width, window.Height),
            };
#endif

            // bind window with UI
            using (var app = new App(window, root))
            {
                window.MessageLoop();
            }
        }
    }
}
