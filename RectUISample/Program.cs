using DesktopDll;
using Graphics;
using RectUI;
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

            var thema = new Thema
            {
                BorderColor = new Color4(0.5f, 0.5f, 0.5f, 1),
                BorderColorHover = new Color4(1, 0, 0, 1),
                FillColor = new Color4(0.8f, 0.8f, 0.8f, 1),
                FillColorHover = new Color4(1, 1, 1, 1),
                FillColorActive = new Color4(1, 1, 0, 1),
            };

            using (var device = D3D11Device.Create())
            using (var swapchain = device.CreateSwapchain(window.WindowHandle))
            using (var backbuffer = swapchain.CreateBitmap())
            {
                var splitter = new HorizontalSplitter(window.Width, window.Height);
                splitter.Add(new RectRegion());
                splitter.Add(new RectRegion());
                var uiContext = new UIContext();

                window.OnResize += (w, h) =>
                {
                    splitter.Rect = new Rect(0, 0, w, h);
                    backbuffer.Dispose();
                    swapchain.Resize(w, h);
                };

                window.OnPaint += () =>
                {
                    backbuffer.Begin(device, new Color4(0.1f, 0.2f, 0.1f, 1.0f));

                    foreach (var d in splitter.Traverse())
                    {
                        backbuffer.DrawRect(device, d.Rect.X, d.Rect.Y, d.Rect.Width, d.Rect.Height,
                            thema.GetFillColor(d),
                            thema.GetBorderColor(d));
                    }

                    backbuffer.End(device);
                    swapchain.Present();
                };

                window.OnMouseLeftDown += (x, y) => uiContext.MouseLeftDown(splitter);
                window.OnMouseLeftUp += (x, y) => uiContext.MouseLeftUp(splitter);
                window.OnMouseRightDown += (x, y) => uiContext.MouseRightDown(splitter);
                window.OnMouseRightUp += (x, y) => uiContext.MouseRightUp(splitter);
                window.OnMouseMiddleDown += (x, y) => uiContext.MouseMiddleDown(splitter);
                window.OnMouseMiddleUp += (x, y) => uiContext.MouseMiddleUp(splitter);
                window.OnMouseMove += (x, y)=> uiContext.MouseMove(splitter, x, y);

                window.MessageLoop();
            }
        }
    }
}
