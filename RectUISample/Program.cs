using DesktopDll;
using RectUI;
using System;
using System.Collections.Generic;

namespace RectUISample
{
    class ListRegion<T>: RectRegion
    {
        int m_height = 18;
        List<T> m_items;
        List<RectRegion> m_regions = new List<RectRegion>();
        public ListRegion(List<T> items)
        {
            m_items = items;
        }

        IEnumerable<RectRegion> Layout()
        {
            var count = Math.Min(m_items.Count, Rect.Height / m_height);
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
                        Drawer = new RectDrawer()
                    };
                    m_regions.Add(r);
                }

                r.Rect = new Rect(Rect.X, y, Rect.Width, m_height);

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
            var root = new ListRegion<string>(new List<string>
            {
                "a", "b", "c",
            })
            {
                Rect = new Rect(window.Width, window.Height),
                Drawer = new RectDrawer(),
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
