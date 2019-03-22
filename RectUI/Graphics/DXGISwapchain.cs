using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;


namespace RectUI.Graphics
{
    public class DXGISwapChain : IDisposable
    {
        SwapChain1 m_swapChain;

        public DXGISwapChain(SwapChain1 swapChain)
        {
            m_swapChain = swapChain;

            var desc = swapChain.Description;
        }

        public void Dispose()
        {
            if (m_swapChain != null)
            {
                m_swapChain.Dispose();
                m_swapChain = null;
            }
        }

        public void Present()
        {
            m_swapChain.Present(0, PresentFlags.None);
        }

        public void Resize(int width, int height)
        {
            if (m_swapChain == null)
            {
                return;
            }
            var desc = m_swapChain.Description;
            m_swapChain.ResizeBuffers(desc.BufferCount, width, height,
                desc.ModeDescription.Format, desc.Flags);
        }

        Texture2D GetBackbuffer()
        {
            return Texture2D.FromSwapChain<Texture2D>(m_swapChain, 0);
        }

        Surface GetSurface()
        {
            return Surface.FromSwapChain(m_swapChain, 0);
        }

        public D3D11RenderTarget CreateRenderTarget()
        {
            return new D3D11RenderTarget(GetBackbuffer);
        }

        public D2D1Bitmap CreateBitmap()
        {
            return new D2D1Bitmap(GetSurface);
        }
    }
}
