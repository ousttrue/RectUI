using DesktopDll;
using RectUI;
using RectUI.Widgets;
using System;
using System.IO;

namespace RectUIGLTF
{
    class FileDialog
    {
        public RectRegion Root;
        public DirSource Source;

        public FileDialog()
        {
            Source = new DirSource();

            Root = new PanelRegion
            {
                new ListRegion<FileSystemInfo>(Source)
                {
                    Anchor= new Anchor
                    {
                        Left=5,
                        Top = 20,
                        Right = 5,
                        Bottom= 36,
                    }
                },

                new ButtonRegion(_ =>
                {

                })
                {
                    Content = "Open",
                    Rect = new Rect(96, 24),
                    Anchor = new Anchor
                    {
                        Bottom=5, Left=5,
                    }
                },

                new ButtonRegion(_ =>
                {

                })
                {
                    Content = "Cancel",
                    Rect = new Rect(96, 24),
                    Anchor = new Anchor
                    {
                        Bottom=5, Right=5,
                    }
                },
            };
        }

        public void Chdir(string path)
        {

        }
    }

    class Program
    {
        static RectRegion BuildUI(Window dialog, FileDialog open)
        {
            return new PanelRegion
            {
                new ButtonRegion((_)=>{
                    Console.WriteLine("clicked");
                    open.Chdir(".");
                    dialog.Show(SW.SHOW);
                })
                {
                    Rect = new Rect(200, 40),
                    Anchor=new Anchor{
                        Left = 5,
                        Top = 5,
                    },
                    Content = "open",
                }
            };
        }

        [STAThread]
        static void Main(string[] args)
        {
            var open = new FileDialog();

            using (var app = new App())
            {
                var main = Window.Create(SW.SHOW);
                var dialog = main.CreateModal(400, 300);

                app.Bind(main, BuildUI(dialog, open));
                app.Bind(dialog, open.Root);

                Window.MessageLoop();
            }
        }
    }
}
