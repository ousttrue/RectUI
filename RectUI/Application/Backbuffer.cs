using DesktopDll;
using RectUI.Graphics;
using SharpDX;
using System;
using System.Collections.Generic;


namespace RectUI.Application
{
    public class Backbuffer : IDisposable
    {
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
        }

        public Backbuffer(D3D11Device device, Window window)
        {
            m_swapchain = device.CreateSwapchain(window.WindowHandle);
            m_backbuffer = m_swapchain.CreateBitmap();
        }

        public void Resize(int w, int h)
        {
            m_backbuffer.Dispose();
            m_swapchain.Resize(w, h);
        }

        public void Paint(D3D11Device device, KeyValuePair<uint, D2DDrawCommand>[] commands)
        {
            m_backbuffer.Begin(device, new Color4(0.1f, 0.2f, 0.1f, 1.0f));
            foreach (var kv in commands)
            {
                m_backbuffer.Draw(device, kv.Key, kv.Value);
            }
            m_backbuffer.End(device);
            m_swapchain.Present();
        }
    }
}
