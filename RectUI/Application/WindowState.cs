using DesktopDll;
using RectUI.Graphics;
using RectUI.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;


namespace RectUI.Application
{
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
            window.OnPaint += Window_OnPaint;
            window.OnMouseLeftDown += Window_OnMouseLeftDown;
            window.OnMouseLeftUp += Window_OnMouseLeftUp;
            window.OnMouseRightDown += Window_OnMouseRightDown;
            window.OnMouseRightUp += Window_OnMouseRightUp;
            window.OnMouseMiddleDown += Window_OnMouseMiddleDown;
            window.OnMouseMiddleUp += Window_OnMouseMiddleUp;
            window.OnMouseMove += Window_OnMouseMove;
            window.OnMouseWheel += Window_OnMouseWheel;
            window.OnMouseLeftDoubleClicked += Window_OnMouseLeftDoubleClicked;

            UIContext.Updated += () => window.Invalidate();

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
        public event Action<D2DDrawCommand[]> OnPaint;

        IEnumerable<D2DDrawCommand> Flatten(RectRegion root)
        {
            foreach (var r in root.Traverse())
            {
                foreach (var c in r.GetDrawCommands(UIContext.Active == r, UIContext.Hover == r))
                {
                    yield return c;
                }
            }
        }

        private void Window_OnPaint()
        {
            var commands = Flatten(m_root).ToArray();
            OnPaint?.Invoke(commands.ToArray());
        }
        #endregion
    }
}
