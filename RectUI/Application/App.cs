using DesktopDll;
using RectUI.Assets;
using RectUI.Graphics;
using RectUI.Widgets;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RectUI.Application
{
    public class App : IDisposable
    {
        RenderThread m_renderThread;
        Dictionary<Window, WindowState> m_windowStateMap = new Dictionary<Window, WindowState>();

        public virtual void Dispose()
        {
            foreach (var kv in m_windowStateMap)
            {
                kv.Value.Dispose();
            }
            m_windowStateMap.Clear();

            if (m_renderThread != null)
            {
                m_renderThread.Dispose();
                m_renderThread = null;
            }
        }

        public App()
        {
            m_renderThread = new RenderThread();
        }

        public void SetAsset(AssetContext asset)
        {
            m_renderThread.EnqueueAsset(asset);
        }

        public void Bind(Window window, RectRegion root)
        {
            var state = new WindowState(window, root);

            m_renderThread.EnqueueWindow(window);

            state.OnPaint += (commands) =>
            {
                m_renderThread.EnqueueCommand(window, commands);
            };
            state.WindowSizeChanged += (w, h) =>
            {
                m_renderThread.EnqueueWindowResize(window, w, h);
            };

            m_windowStateMap.Add(window, state);

            window.OnDestroy += () => Window_OnDestroy(window);
        }

        private void Window_OnDestroy(Window window)
        {
            m_renderThread.EnqueueWindowDestroy(window);

            WindowState state;
            if (!m_windowStateMap.TryGetValue(window, out state))
            {
                return;
            }
            state.Dispose();
            m_windowStateMap.Remove(window);

            if (!m_windowStateMap.Any())
            {
                User32.PostQuitMessage(0);
            }
        }

        public void Draw()
        {
            foreach(var kv in m_windowStateMap)
            {
                kv.Value.Draw();
            }
        }
    }
}
