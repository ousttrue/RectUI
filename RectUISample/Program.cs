using DesktopDll;
using RectUI;
using System;
using System.IO;
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
            var dir = new DirSource()
            {
            };
            var left = new ListRegion<FileSystemInfo>(dir)
            {
                Rect = new Rect(window.Width, window.Height),
            };
            left.ItemGetDrawCommands = (uiContext, i, r) =>
            {
                var rect = DrawCommandFactory.DrawRectCommands(uiContext, r);
                var label = r.Content.ToString();
                if (dir.Current.Parent.FullName == r.Content.FullName)
                {
                    label = "..";
                }
                var text = DrawCommandFactory.DrawTextCommands(uiContext, r, "MS Gothic", 
                    left.ItemHeight, 
                    5, 3, 5, 2,
                    label);
                return rect.Concat(text);
            };
            left.ItemLeftClicked += (i, content) =>
              {
                  var d = content as DirectoryInfo;
                  if(d!=null)
                  {
                      dir.ChangeDirectory(d);
                  }
                  else
                  {
                      Console.WriteLine($"{i}:{content}");
                  }
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
