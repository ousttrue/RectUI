using DesktopDll;
using RectUI.Assets;
using RectUI.Graphics;
using RectUI.JSON;
using System;
using SharpDX;

namespace RectUI.Application
{
    public class Backbuffer : IDisposable
    {
        DXGISwapChain m_swapchain;
        D2D1Bitmap m_backbuffer;
        Color4 m_clear = new Color4(0.1f, 0.2f, 0.1f, 1.0f);

        Dispatcher m_dispatcher = new Dispatcher();

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

            m_dispatcher.RegisterInterface(typeof(IDrawProcessor), m_backbuffer);
        }

        public void Resize(int w, int h)
        {
            m_backbuffer.Dispose();
            m_swapchain.Resize(w, h);
        }

        static ArraySegment<byte> Skip(ArraySegment<byte> src, int skip)
        {
            return new ArraySegment<byte>(src.Array, src.Offset + skip, src.Count - skip);
        }

        ListTreeNode<MsgPackValue> m_parsed = new ListTreeNode<MsgPackValue>();

        public void RenderMsgPackCommands(D3D11Device device,
            Scene scene,
            ArraySegment<byte> commands)
        {
            m_backbuffer.Begin(device, scene, m_clear);
            while (commands.Count > 0)
            {
                try
                {
                    m_parsed.Clear();
                    var parsed = MsgPackParser.Parse(m_parsed, commands);

                    m_dispatcher.Dispatch(parsed);

                    commands = Skip(commands, parsed.Value.Bytes.Count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    break;
                }
            }
            m_backbuffer.End();
            m_swapchain.Present();
        }
    }
}
