using DesktopDll;
using Graphics;
using SharpDX;
using System;


namespace SimpleDX
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var window = Window.Create();
            if (window == null)
            {
                return;
            }

            window.Show();

            using (var device = D3D11Device.Create())
            using (var swapchain = device.CreateSwapchain(window.WindowHandle))
            using (var backbuffer = swapchain.CreateBitmap())
            {
                window.OnResize += (w, h) =>
                {
                    backbuffer.Dispose();
                    swapchain.Resize(w, h);
                };

                var splitter = new HorizontalSplitter(window.Width, window.Height);
                splitter.Add(new RectRegion());

                window.OnPaint += () =>
                {
                    backbuffer.Begin(device, new Color4(0.1f, 0.2f, 0.1f, 0));
                    
                    // ToDo draw splitter
                    foreach(var d in splitter.Traverse())
                    {
                        backbuffer.DrawRect(device, d.Rect.X, d.Rect.Y, d.Rect.Width, d.Rect.Height, Color.White);
                    }

                    backbuffer.End(device);
                    swapchain.Present();

                    Console.WriteLine("Draw");
                };

                window.OnMouseMove += (x, y) =>
                {
                    splitter.MouseMove(x, y);
                };

                window.MessageLoop();
            }
        }
    }
}
