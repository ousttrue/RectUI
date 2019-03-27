using DesktopDll;
using RectUI.Graphics;
using RectUI.Widgets;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RectUI
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

            UIContext.Updated += () => window.Invalidate();

            m_swapchain = device.CreateSwapchain(window.WindowHandle);
            m_backbuffer = m_swapchain.CreateBitmap();
            m_root = root;

            Window_OnResize(window.Width, window.Height);

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

        private void Window_OnPaint()
        {
            m_backbuffer.Begin(m_device, new Color4(0.1f, 0.2f, 0.1f, 1.0f));

            foreach (var r in m_root.Traverse())
            {
                foreach (var c in r.GetDrawCommands(UIContext.Active==r, UIContext.Hover==r).SelectMany(x => x))
                {
                    m_backbuffer.Draw(m_device, c);
                }
            }

            m_backbuffer.End(m_device);
            m_swapchain.Present();
        }

        private void Window_OnResize(int w, int h)
        {
            m_root.Rect = new Rect(0, 0, w, h);
            m_backbuffer.Dispose();
            m_swapchain.Resize(w, h);
        }
    }


    public class App : IDisposable
    {
        D3D11Device m_device;

        Dictionary<Window, WindowState> m_windowStateMap = new Dictionary<Window, WindowState>();

        public virtual void Dispose()
        {
            foreach (var kv in m_windowStateMap)
            {
                kv.Value.Dispose();
            }
            m_windowStateMap.Clear();

            if (m_device != null)
            {
                m_device.Dispose();
                m_device = null;
            }
        }

        public App()
        {
            m_device = D3D11Device.Create();
        }

        public void Bind(Window window, RectRegion root)
        {
            m_windowStateMap.Add(window, new WindowState(m_device, window, root));

            window.OnDestroy += () => Window_OnDestroy(window);
        }

        private void Window_OnDestroy(Window window)
        {
            m_windowStateMap.Remove(window);
            if (!m_windowStateMap.Any())
            {
                User32.PostQuitMessage(0);
            }
        }
    }
}
