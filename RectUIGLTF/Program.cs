using DesktopDll;
using RectUI;
using System;


namespace RectUIGLTF
{
    class Program
    {
        static RectRegion BuildFileDialog()
        {
            return new PanelRegion
            {
            };
        }

        static RectRegion BuildUI(Window dialog)
        {
            return new PanelRegion
            {
                new ButtonRegion((_)=>{
                    Console.WriteLine("clicked");
                    dialog.Show(SW.SHOW);
                })
                {
                    Anchor=new Anchor{
                        Left = 5,
                        Top = 5,
                        Right = 5,
                        Bottom = 5,
                    },
                    Content = "open",
                }
            };
        }

        [STAThread]
        static void Main(string[] args)
        {
            using (var app = new App())
            {
                var main = Window.Create(SW.SHOW);
                var dialog = main.CreateModal(400, 300);

                app.Bind(dialog, BuildFileDialog());
                app.Bind(main, BuildUI(dialog));
                   
                Window.MessageLoop();
            }
        }
    }
}
