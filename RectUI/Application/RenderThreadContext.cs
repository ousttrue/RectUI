using DesktopDll;
using RectUI.Assets;
using RectUI.Graphics;
using System;
using System.Collections.Generic;


namespace RectUI.Application
{
    /// <summary>
    /// スレッド上からのみアクセス可能。キューを介してのみアクセスできる
    /// </summary>
    class Context : IDisposable
    {
        D3D11Device m_device;

        Scene m_scene = new Scene();
        public Scene Scene
        {
            get
            {
                return m_scene;
            }
        }

        public void Dispose()
        {
            if (m_scene != null)
            {
                m_scene.Dispose();
                m_scene = null;
            }

            if (m_device != null)
            {
                m_device.Dispose();
                m_device = null;
            }
        }

        public Context()
        {
            m_device = D3D11Device.Create();
        }

        Dictionary<Window, Backbuffer> m_bbMap = new Dictionary<Window, Backbuffer>();

        public void AddWindow(Window window)
        {
            var bb = new Backbuffer(m_device, window);
            m_bbMap.Add(window, bb);
        }

        public void ResizeWindow(Window target, int w, int h)
        {
            Backbuffer bb;
            if (!m_bbMap.TryGetValue(target, out bb))
            {
                return;
            }
            bb.Resize(w, h);
        }

        public void DestroyWindow(Window target)
        {
            Backbuffer bb;
            if (!m_bbMap.TryGetValue(target, out bb))
            {
                return;
            }
            m_bbMap.Remove(target);
            bb.Dispose();
        }

        public void Draw(Window target, ArraySegment<byte> commands)
        {
            Backbuffer bb;
            if (!m_bbMap.TryGetValue(target, out bb))
            {
                return;
            }
            bb.RenderMsgPackCommands(m_device, m_scene, commands);
        }
    }
}
