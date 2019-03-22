using DesktopDll;
using RectUI;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace RectUISample
{
    class SystemIcon: IDisposable
    {
        public HICON Icon
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (Icon.Value != IntPtr.Zero)
            {
                User32.DestroyIcon(Icon);
                Icon = new HICON { Value = IntPtr.Zero };
            }
        }

        SystemIcon(IntPtr himl, int i, IDL idl)
        {
            Icon = Comctl32.ImageList_GetIcon(himl, i, idl);
        }

        public static SystemIcon Get(string path, bool isSmall)
        {
            var flags = SHGFI.SYSICONINDEX;
            if (isSmall)
            {
                flags |= SHGFI.SMALLICON;
            }
            var sfi = default(SHFILEINFOW);
            var result = Shell32.SHGetFileInfoW(path, -1,
                ref sfi, Marshal.SizeOf<SHFILEINFOW>(),
                flags);
            if (result == IntPtr.Zero)
            {
                return null;
            }

            return new SystemIcon(result, sfi.iIcon, IDL.NORMAL);
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
                var commands = DrawCommandFactory.DrawRectCommands(uiContext, r);
                var label = r.Content.ToString();
                if (dir.Current.Parent.FullName == r.Content.FullName)
                {
                    label = "..";
                }

                using (var icon = SystemIcon.Get(r.Content.FullName, true))
                {
                    commands.Concat(DrawCommandFactory.DrawIconCommands(uiContext, r, icon.Icon));
                }

                var text = DrawCommandFactory.DrawTextCommands(uiContext, r, "MS Gothic", 
                    left.ItemHeight, 
                    5, 3, 5, 2,
                    label);
                commands = commands.Concat(text);

                return commands;
            };
            left.ItemLeftClicked += (i, content) =>
              {
                  var d = content as DirectoryInfo;
                  if(d!=null)
                  {
                      dir.ChangeDirectory(d);
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
