using DesktopDll;
using Graphics;
using RectUI;
using SharpDX;
using System;


namespace RectUISample
{
    class App : IDisposable
    {
        D3D11Device m_device;
        DXGISwapChain m_swapchain;
        D2D1Bitmap m_backbuffer;
        public void Dispose()
        {
            if (m_backbuffer != null)
            {
                m_backbuffer.Dispose();
                m_backbuffer = null;
            }
            if (m_swapchain != null)
            {
                m_swapchain.Dispose();
                m_swapchain = null;
            }
            if (m_device != null)
            {
                m_device.Dispose();
                m_device = null;
            }
        }

        HorizontalSplitter m_splitter;
        UIContext m_uiContext = new UIContext();
        Thema m_thema = new Thema
        {
            BorderColor = new Color4(0.5f, 0.5f, 0.5f, 1),
            BorderColorHover = new Color4(1, 0, 0, 1),
            FillColor = new Color4(0.8f, 0.8f, 0.8f, 1),
            FillColorHover = new Color4(1, 1, 1, 1),
            FillColorActive = new Color4(1, 1, 0, 1),
        };

        public App(Window window)
        {
            m_device = D3D11Device.Create();
            m_swapchain = m_device.CreateSwapchain(window.WindowHandle);
            m_backbuffer = m_swapchain.CreateBitmap();

            m_splitter = new HorizontalSplitter(window.Width, window.Height);
            m_splitter.Add(new RectRegion());
            m_splitter.Add(new RectRegion());

            window.OnResize += Window_OnResize;
            window.OnPaint += Window_OnPaint;
            window.OnMouseLeftDown += Window_OnMouseLeftDown;
            window.OnMouseLeftUp += Window_OnMouseLeftUp;
            window.OnMouseRightDown += Window_OnMouseRightDown;
            window.OnMouseRightUp += Window_OnMouseRightUp;
            window.OnMouseMiddleDown += Window_OnMouseMiddleDown;
            window.OnMouseMiddleUp += Window_OnMouseMiddleUp;
            window.OnMouseMove += Window_OnMouseMove;
        }

        private void Window_OnMouseMove(int x, int y)
        {
            m_uiContext.MouseMove(m_splitter, x, y);
        }

        private void Window_OnMouseMiddleUp(int _x, int _y)
        {
            m_uiContext.MouseMiddleUp(m_splitter);
        }

        private void Window_OnMouseMiddleDown(int _x, int _y)
        {
            m_uiContext.MouseMiddleDown(m_splitter);
        }

        private void Window_OnMouseRightUp(int _x, int _y)
        {
            m_uiContext.MouseRightUp(m_splitter);
        }

        private void Window_OnMouseRightDown(int arg1, int arg2)
        {
            m_uiContext.MouseRightDown(m_splitter);
        }

        private void Window_OnMouseLeftUp(int _x, int _y)
        {
            m_uiContext.MouseLeftUp(m_splitter);
        }

        private void Window_OnMouseLeftDown(int _x, int _y)
        {
            m_uiContext.MouseLeftDown(m_splitter);
        }

        private void Window_OnPaint()
        {
            m_backbuffer.Begin(m_device, new Color4(0.1f, 0.2f, 0.1f, 1.0f));

            foreach (var r in m_splitter.Traverse())
            {
                m_backbuffer.DrawRect(m_device, r.Rect.X, r.Rect.Y, r.Rect.Width, r.Rect.Height,
                    m_thema.GetFillColor(m_uiContext, r),
                    m_thema.GetBorderColor(m_uiContext, r));
            }

            m_backbuffer.End(m_device);
            m_swapchain.Present();
        }

        private void Window_OnResize(int w, int h)
        {
            m_splitter.Rect = new Rect(0, 0, w, h);
            m_backbuffer.Dispose();
            m_swapchain.Resize(w, h);
        }
    }
}
