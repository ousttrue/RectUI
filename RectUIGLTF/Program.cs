using DesktopDll;
using RectUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectUIGLTF
{
    class GLTFApp : App
    {
        public GLTFApp(Window window) : base(window)
        {
        }

        protected override RectRegion BuildUI(Window window)
        {
            return new RectRegion();
        }
    }

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // create window
            var window = Window.Create();
            window.Show();

            // bind window with UI
            using (var app = new GLTFApp(window))
            {
                window.MessageLoop();
            }
        }
    }
}
