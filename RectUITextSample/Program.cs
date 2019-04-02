using DesktopDll;
using RectUI.Application;
using RectUI.Widgets;
using System;


namespace RectUITextSample
{
    class Program
    {
        static RectRegion BuildUI()
        {
            return new RectRegion();
        }

        [STAThread]
        static void Main(string[] args)
        {
            using (var app = new App())
            {
                app.Bind(Window.Create(), BuildUI());

                MessageLoop.Run(()=>app.Draw(), 30);
            }
        }
    }
}
