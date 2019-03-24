using DesktopDll;
using RectUI;
using System;


namespace RectUIGLTF
{
    class Program
    {
        static RectRegion BuildUI()
        {
            return new PanelRegion
            {
                new ButtonRegion((_)=>Console.WriteLine("clicked"))
                {
                    Rect = new Rect(5, 5, 200, 100),
                    Content = "open",
                }
            };
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
