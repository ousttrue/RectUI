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
            using (var sc = device.CreateSwapchain(window.WindowHandle))
            using (var bb = sc.CreateRenderTarget())
            {
                window.OnResize += (w, h) =>
                {
                    bb.Dispose();
                    sc.Resize(w, h);
                };

                window.OnPaint += () =>
                {
                    bb.Setup(device, new Color4(0.1f, 0.2f, 0.1f, 0));
                    sc.Present();
                };

                while (window.MessageLoop())
                {

                }
            }
        }
    }
}
