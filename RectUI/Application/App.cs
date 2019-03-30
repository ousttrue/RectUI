using DesktopDll;
using RectUI.Assets;
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

        struct WindowBuffer
        {
            public WindowState State;
            public Backbuffer Buffer;
        }
        Dictionary<Window, WindowBuffer> m_windowStateMap = new Dictionary<Window, WindowBuffer>();

        Scene m_scene = new Scene();
        public Scene Scene
        {
            get { return m_scene; }
        }

        public virtual void Dispose()
        {
            if (m_scene != null)
            {
                m_scene.Dispose();
                m_scene = null;
            }

            foreach (var kv in m_windowStateMap)
            {
                kv.Value.State.Dispose();
                kv.Value.Buffer.Dispose();
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
            var state = new WindowState(window, root);
            var bb = new Backbuffer(m_device, window);

            state.OnPaint += (commands) =>
            {
                bb.ExecuteCommands(m_device, m_scene, commands);
            };
            state.WindowSizeChanged += (w, h) => bb.Resize(w, h);

            m_windowStateMap.Add(window, new WindowBuffer
            {
                State = state,
                Buffer =bb,
            });

            window.OnDestroy += () => Window_OnDestroy(window);
        }

        private void Window_OnDestroy(Window window)
        {
            WindowBuffer buffer;
            if (!m_windowStateMap.TryGetValue(window, out buffer))
            {
                return;
            }
            buffer.State.Dispose();
            buffer.Buffer.Dispose();
            m_windowStateMap.Remove(window);

            if (!m_windowStateMap.Any())
            {
                User32.PostQuitMessage(0);
            }
        }
    }
}
