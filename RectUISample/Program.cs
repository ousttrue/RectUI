using DesktopDll;
using RectUI;
using System;


namespace RectUISample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var window = Window.Create();
            window.Show();

            var root = new HorizontalSplitter(window.Width, window.Height);
            root.Add(new RectRegion());
            root.Add(new RectRegion());

            using (var app = new App(window, root))
            {
                window.MessageLoop();
            }
        }
    }
}
