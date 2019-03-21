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
            var root = new ListRegion(new DirSource())
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
