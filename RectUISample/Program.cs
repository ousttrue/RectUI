using DesktopDll;
using RectUI;
using RectUI.Widgets;
using System;
using System.IO;


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
