﻿using DesktopDll;
using RectUI;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace RectUISample
{
    class SystemIcon
    {
        public IntPtr ImageList
        {
            get;
            private set;
        }

        public int ImageListIndex
        {
            get;
            private set;
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

            return new SystemIcon
            {
                ImageList = result,
                ImageListIndex = sfi.iIcon,
            };
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
                var rect = r.Rect.ToSharpDX();
                rect.X += 16;
                rect.Width -= 16;
                var commands = DrawCommandFactory.DrawRectCommands(rect, 
                    Style.Default.GetFillColor(uiContext, r),
                    Style.Default.GetBorderColor(uiContext, r));
                var label = r.Content.ToString();
                if (dir.Current.Parent.FullName == r.Content.FullName)
                {
                    label = "..";
                }

                var icon = SystemIcon.Get(r.Content.FullName, true);
                commands = commands.Concat(DrawCommandFactory.DrawImageListCommands(uiContext, r,
                    icon.ImageList, icon.ImageListIndex));

                var text = DrawCommandFactory.DrawTextCommands(uiContext, r, "MS Gothic", 
                    left.ItemHeight, 
                    21, 3, 5, 2,
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
            root.Add(left);
            // right
            root.Add(new RectRegion());

            // bind window with UI
            using (var app = new App(window, root))
            {
                window.MessageLoop();
            }
        }
    }
}
