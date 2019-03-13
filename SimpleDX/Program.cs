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
            using (var backbuffer = swapchain.CreateRenderTarget())
            {
                window.OnResize += (w, h) =>
                {
                    backbuffer.Dispose();
                    swapchain.Resize(w, h);
                };

                var splitter = new HorizontalSplitter(window.Width, window.Height);

                window.OnPaint += () =>
                {
                    backbuffer.Setup(device, new Color4(0.1f, 0.2f, 0.1f, 0));

                    // ToDo draw splitter

                    swapchain.Present();
                };

                window.OnMouseMove += (x, y) =>
                {
                    //Console.WriteLine($"{x}, {y}");
                    splitter.MouseMove(x, y);
                };

                window.MessageLoop();
            }
        }
    }
}
