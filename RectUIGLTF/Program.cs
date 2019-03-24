using DesktopDll;
using RectUI;
using System;


namespace RectUIGLTF
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
                   
                Window.MessageLoop();
            }
        }
    }
}
