using DesktopDll;
using RectUI.Graphics;
using RectUI.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;



namespace RectUI.Application
{
    public class WindowState : IDisposable
    {
        D3D11Device m_device;
        DXGISwapChain m_swapchain;
        D2D1Bitmap m_backbuffer;
        RectRegion m_root;

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
        }

        public UIContext UIContext
        {
            get;
            private set;
        }

        public WindowState(D3D11Device device, Window window, RectRegion root)
        {
            m_device = device;

            UIContext = new UIContext();

            window.OnResize += Window_OnResize;
            window.OnPaint += Window_OnPaint;
            window.OnMouseLeftDown += Window_OnMouseLeftDown;
            window.OnMouseLeftUp += Window_OnMouseLeftUp;
            window.OnMouseRightDown += Window_OnMouseRightDown;
            window.OnMouseRightUp += Window_OnMouseRightUp;
            window.OnMouseMiddleDown += Window_OnMouseMiddleDown;
            window.OnMouseMiddleUp += Window_OnMouseMiddleUp;
            window.OnMouseMove += Window_OnMouseMove;
            window.OnMouseWheel += Window_OnMouseWheel;
            window.OnMouseLeftDoubleClicked += Window_OnMouseLeftDoubleClicked;

            UIContext.Updated += () => window.Invalidate();

            m_swapchain = device.CreateSwapchain(window.WindowHandle);
            m_backbuffer = m_swapchain.CreateBitmap();
            m_root = root;

            Window_OnResize(window.Width, window.Height);

        }

        private void Window_OnMouseLeftDoubleClicked()
        {
            UIContext.MouseLeftDoubleClicked();
        }

        private void Window_OnMouseWheel(int delta)
        {
            UIContext.MouseWheel(delta);
        }

        private void Window_OnMouseMove(int x, int y)
        {
            UIContext.MouseMove(m_root, x, y);
        }

        private void Window_OnMouseMiddleUp(int x, int y)
        {
            UIContext.MouseMiddleUp(x, y);
        }

        private void Window_OnMouseMiddleDown(int x, int y)
        {
            UIContext.MouseMiddleDown(x, y);
        }

        private void Window_OnMouseRightUp(int x, int y)
        {
            UIContext.MouseRightUp(x, y);
        }

        private void Window_OnMouseRightDown(int x, int y)
        {
            UIContext.MouseRightDown(x, y);
        }

        private void Window_OnMouseLeftUp(int x, int y)
        {
            UIContext.MouseLeftUp(x, y);
        }

        private void Window_OnMouseLeftDown(int x, int y)
        {
            UIContext.MouseLeftDown(x, y);
        }

        public event Action<D2D1Bitmap, DXGISwapChain, KeyValuePair<uint, D2DDrawCommand>[]> OnPaint;

        IEnumerable<KeyValuePair<uint, D2DDrawCommand>> Flatten(RectRegion root)
        {
            foreach (var r in root.Traverse())
            {
                foreach (var commands in r.GetDrawCommands(UIContext.Active == r, UIContext.Hover == r))
                {
                    foreach (var c in commands)
                    {
                        yield return new KeyValuePair<uint, D2DDrawCommand>(r.ID, c);
                    }
                }
            }
        }

        private void Window_OnPaint()
        {
            var commands = Flatten(m_root).ToArray();
            OnPaint?.Invoke(m_backbuffer, m_swapchain, commands.ToArray());
        }

        private void Window_OnResize(int w, int h)
        {
            m_root.Rect = new Rect(0, 0, w, h);
            m_backbuffer.Dispose();
            m_swapchain.Resize(w, h);
        }
    }


}
