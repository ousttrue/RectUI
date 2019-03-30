using DesktopDll;
using RectUI.Assets;
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
        D3D11RenderTarget m_renderTarget;

        public void Dispose()
        {
            if (m_renderTarget != null)
            {
                m_renderTarget.Dispose();
                m_renderTarget = null;
            }

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

        RectangleF m_rect;
        D3D11RenderTarget GetOrRenderTarget(D3D11Device device, uint id, RectangleF rect)
        {
            if (rect != m_rect)
            {
                m_rect = rect;
                if (m_renderTarget != null)
                {
                    m_renderTarget.Dispose();
                    m_renderTarget = null;
                }
            }
            if (m_renderTarget == null)
            {
                m_renderTarget = D3D11RenderTarget.Create(device, (int)rect.Width, (int)rect.Height);
            }
            return m_renderTarget;
        }

        public void ExecuteCommands(D3D11Device device,
            Scene scene,
            D2DDrawCommand[] commands)
        {
            m_backbuffer.Begin(device, new Color4(0.1f, 0.2f, 0.1f, 1.0f));
            foreach (var command in commands)
            {
                switch (command.DrawType)
                {
                    case DrawType.Rectangle:
                        m_backbuffer.DrawRect(device, command.Rectangle,
                            command.FillColor, command.BorderColor);
                        break;

                    case DrawType.Text:
                        m_backbuffer.DrawText(device, command.Rectangle,
                            command.Font,
                            command.TextColor, command.Text);
                        break;

                    case DrawType.Icon:
                        m_backbuffer.DrawIcon(device, command.Rectangle,
                            command.Icon);
                        break;

                    case DrawType.ImageList:
                        m_backbuffer.DrawImageList(device, command.Rectangle,
                            command.Icon, command.ImageListIndex);
                        break;

                    case DrawType.Scene:
                        {
                            // render 3D scene
                            var m_renderTarget = GetOrRenderTarget(device, command.RegionID, command.Rectangle);
                            m_renderTarget.Setup(device, new Color4(0.2f, 0, 0, 1));
                            device.SetViewport(new Viewport(0, 0,
                                (int)command.Rectangle.Width,
                                (int)command.Rectangle.Height));
                            scene.Draw(device, command.Camera.ViewProjection);

                            m_backbuffer.DrawRenderTarget(device, command.Rectangle, m_renderTarget);
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            m_backbuffer.End(device);
            m_swapchain.Present();
        }
    }
}
