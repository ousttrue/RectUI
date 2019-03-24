using DesktopDll;
using RectUI;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace RectUISample
{
    class Program
    {
        static RectRegion BuildUI()
        {
            // build UI
            var root = new HorizontalSplitter
            {
                Rect = new Rect()
            };
            // left
            var dir = new DirSource()
            {
            };
            var left = new ListRegion<FileSystemInfo>(dir)
            {
                Rect = new Rect(),
            };
            left.ItemLeftClicked += (i, content) =>
            {
                var d = content as DirectoryInfo;
                if (d != null)
                {
                    dir.ChangeDirectory(d);
                }
            };
            left.OnWheel += (_, delta) =>
            {
                //Console.WriteLine(delta);
                if (delta < 0)
                {
                    left.ScrollY += left.ItemHeight * 2;
                }
                else
                {
                    left.ScrollY -= left.ItemHeight * 2;
                }
            };
            root.Left = left;
            // right
            root.Right = new RectRegion();

            return root;
        }

        [STAThread]
        static void Main(string[] args)
        {
            using (var app = new App())
            {
                app.Bind(Window.Create(), BuildUI());

                Window.MessageLoop();
            }
        }
    }
}
