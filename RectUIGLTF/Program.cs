using DesktopDll;
using RectUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
            // create window
            var window = Window.Create();
            window.Show();

            // bind window with UI
            using (var app = new App())
            {
                app.Bind(window, BuildUI());
                   
                Window.MessageLoop();
            }
        }
    }
}
