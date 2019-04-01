using DesktopDll;
using RectUI.Graphics;
using RectUI.Widgets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace RectUI.Application
{
    public class CommandList
    {
        RpcDrawProcessorBuffer m_rpc = new RpcDrawProcessorBuffer();
        public RpcDrawProcessorBuffer Rpc
        {
            get { return m_rpc; }
        }

        Action<CommandList> m_release;

        public CommandList(Action<CommandList> release)
        {
            m_release = release;
        }

        public void Release()
        {
            m_release(this);
        }
    }

    public class WindowState : IDisposable
    {
        RectRegion m_root;

        public void Dispose()
        {
            if (m_root != null)
            {
                m_root.Dispose();
                m_root = null;
            }
        }

        public UIContext UIContext
        {
            get;
            private set;
        }

        public WindowState(Window window, RectRegion root)
        {
            m_root = root;
            UIContext = new UIContext();

            window.OnResize += Window_OnResize;
            window.OnPaint += () => m_invalidated = true;
            window.OnMouseLeftDown += Window_OnMouseLeftDown;
            window.OnMouseLeftUp += Window_OnMouseLeftUp;
            window.OnMouseRightDown += Window_OnMouseRightDown;
            window.OnMouseRightUp += Window_OnMouseRightUp;
            window.OnMouseMiddleDown += Window_OnMouseMiddleDown;
            window.OnMouseMiddleUp += Window_OnMouseMiddleUp;
            window.OnMouseMove += Window_OnMouseMove;
            window.OnMouseWheel += Window_OnMouseWheel;
            window.OnMouseLeftDoubleClicked += Window_OnMouseLeftDoubleClicked;

            //UIContext.Updated += () => window.Invalidate();
            UIContext.Updated += () => m_invalidated = true;

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

        public event Action<int, int> WindowSizeChanged;
        private void Window_OnResize(int w, int h)
        {
            m_root.Rect = new Rect(0, 0, w, h);
            WindowSizeChanged?.Invoke(w, h);
        }

        #region OnPaing
        public event Action<CommandList> OnPaint;

        Queue<CommandList> m_pool = new Queue<CommandList>();

        public void ReleaseList(CommandList list)
        {
            list.Rpc.Clear();
            lock (((ICollection)m_pool).SyncRoot)
            {
                m_pool.Enqueue(list);
            }
        }

        CommandList GetList()
        {
            lock (((ICollection)m_pool).SyncRoot)
            {
                if (m_pool.Count > 0)
                {
                    return m_pool.Dequeue();
                }
            }
            return new CommandList(ReleaseList);
        }

        bool m_invalidated;
        public void Draw()
        {
            if (m_invalidated)
            {
                m_invalidated = false;
                Window_OnPaint();
            }
        }

        private void Window_OnPaint()
        {
            var list = GetList();

            foreach (var r in m_root.Traverse())
            {
                r.GetDrawCommands(list.Rpc, UIContext.Active == r, UIContext.Hover == r);
            }

            OnPaint?.Invoke(list);
        }
        #endregion
    }
}
