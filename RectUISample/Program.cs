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
            var root = new HorizontalSplitter
            {
                Left = new ListRegion<FileSystemInfo>(new DirSource()),
                Right = new RectRegion(),
            };
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
