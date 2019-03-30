using DesktopDll;
using RectUI.Graphics;
using RectUI.Widgets;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RectUI.Application
{
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
            var state = new WindowState(m_device, window, root);
            state.OnPaint += State_OnPaint;
            m_windowStateMap.Add(window, state);

            window.OnDestroy += () => Window_OnDestroy(window);
        }

        private void State_OnPaint(D2D1Bitmap m_backbuffer, DXGISwapChain m_swapchain, 
            KeyValuePair<uint, D2DDrawCommand>[] commands)
        {
            m_backbuffer.Begin(m_device, new Color4(0.1f, 0.2f, 0.1f, 1.0f));
            foreach (var kv in commands)
            {
                m_backbuffer.Draw(m_device, kv.Key, kv.Value);
            }       
            m_backbuffer.End(m_device);
            m_swapchain.Present();
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
