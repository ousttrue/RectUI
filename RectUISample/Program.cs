using DesktopDll;
using System;


namespace RectUISample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var window = Window.Create();
            window.Show();
            using (var app = new App(window))
            {
                window.MessageLoop();
            }
        }
    }
}
