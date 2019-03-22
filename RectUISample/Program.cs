using DesktopDll;
using RectUI;
using System;
using System.Linq;


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
            var root = new HorizontalSplitter
            {
                Rect = new Rect(window.Width, window.Height)
            };
            // left
            var left = new ListRegion<string>(new DirSource())
            {
                Rect = new Rect(window.Width, window.Height),
            };
            left.ItemGetDrawCommands = (uiContext, i, r) =>
            {
                var rect = DrawCommandFactory.DrawRectCommands(uiContext, r);
                var text = DrawCommandFactory.DrawTextCommands(uiContext, r, "MS Gothic", 
                    left.ItemHeight, 
                    5, 3, 5, 2,
                    r.Content);
                return rect.Concat(text);
            };
            left.ItemLeftClicked += (i, content) =>
              {
                  Console.WriteLine($"{i}:{content}");
              };
            root.Add(left);
            // right
            var right = new RectRegion
            {
                OnGetDrawCommands = (uiContext, r) =>
                {
                    return DrawCommandFactory.DrawRectCommands(uiContext, r);
                },
            };
            root.Add(right);

            // bind window with UI
            using (var app = new App(window, root))
            {
                window.MessageLoop();
            }
        }
    }
}
