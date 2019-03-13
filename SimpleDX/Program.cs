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

                window.OnPaint += () =>
                {
                    backbuffer.Setup(device, new Color4(0.1f, 0.2f, 0.1f, 0));
                    swapchain.Present();
                };

                window.OnMouseMove += (x, y) =>
                {
                    //Console.WriteLine($"{x}, {y}");
                };

                while (window.MessageLoop())
                {

                }
            }
        }
    }
}
