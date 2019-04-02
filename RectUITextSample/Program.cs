using DesktopDll;
using RectUI.Application;
using RectUI.Graphics;
using RectUI.Widgets;
using System;


namespace RectUITextSample
{
    class Program
    {
        static RectRegion BuildUI()
        {
            var fs = new FontSource();

            return new HorizontalSplitter
            {           
               Left = new ListRegion<FontFaceName>(fs),
               Right = new TextRegion()
               {
                   Text = Lorem.Text,
               },
            };
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
