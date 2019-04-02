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

            var tr = new TextRegion()
            {
                Text = Lorem.Text,
            };

            var list = new ListRegion<FontFaceName>(fs);

            list.SelectionChanged += () =>
            {
                tr.Style.Font = list.Selected;
            };

            return new HorizontalSplitter
            {           
               Left = list,
               Right = tr,
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
