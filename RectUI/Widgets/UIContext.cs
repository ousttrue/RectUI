using System;
using System.Linq;


namespace RectUI.Widgets
{
    /// <summary>
    /// 
    /// </summary>
    public class UIContext
    {
        RectRegion _mouseLeftDown;
        RectRegion _mouseRightDown;
        RectRegion _mouseMiddleDown;
        public bool IsMouseLeftDown => _mouseLeftDown != null;
        public bool IsMouseRightDown => _mouseRightDown != null;
        public bool IsMouseMiddleDown => _mouseMiddleDown != null;
        public bool IsAnyDown
        {
            get { return IsMouseLeftDown | IsMouseRightDown | IsMouseMiddleDown; }
        }
        public int MouseX;
        public int MouseY;

        /// <summary>
        /// When Mouse over
        /// </summary>
        RectRegion _hover;
        public RectRegion Hover
        {
            get { return _hover; }
            set
            {
                if (_hover == value) return;
                _hover = value;
                Updated?.Invoke();
            }
        }

        /// <summary>
        /// When Drag any
        /// </summary>
        RectRegion _active;
        public RectRegion Active
        {
            get { return _active; }
            set
            {
                if (_active == value) return;
                _active = value;
                Updated?.Invoke();
            }
        }

        public event Action Updated;

        public void MouseMove(RectRegion root, int x, int y)
        {
            MouseX = x;
            MouseY = y;
            Hover = root.MouseMove(MouseX, MouseY);

            if (Active != null)
            {
                if (_mouseLeftDown != null)
                {
                    if (_mouseLeftDown.MouseLeftDrag(Active, DragEvent.Drag, x, y))
                    {
                        Updated?.Invoke();
                    }
                }
                if (_mouseRightDown != null)
                {
                    if (_mouseRightDown.MouseRightDrag(Active, DragEvent.Drag, x, y))
                    {
                        Updated?.Invoke();
                    }
                }
                if (_mouseMiddleDown != null)
                {
                    if (_mouseMiddleDown.MouseMiddleDrag(Active, DragEvent.Drag, x, y))
                    {
                        Updated?.Invoke();
                    }
                }
            }
        }

        public void MouseLeftDown(int x, int y)
        {
            if (Active == null)
            {
                Active = Hover;
            }
            _mouseLeftDown = Active.ParentPath.FirstOrDefault(r => r.MouseLeftDrag(r, DragEvent.Begin, x, y));
        }

        public void MouseLeftUp(int x, int y)
        {
            if (Active != null)
            {
                if (Active == Hover)
                {
                    Active.MouseLeftClick(Active);
                }

                if (_mouseLeftDown != null)
                {
                    _mouseLeftDown.MouseLeftDrag(Active, DragEvent.End, x, y);
                    _mouseLeftDown = null;
                }
            }
            if (!IsAnyDown)
            {
                Active = null;
            }
        }

        public void MouseRightDown(int x, int y)
        {
            if (Active == null)
            { 
                Active = Hover;
            }
            _mouseRightDown = Active.ParentPath.FirstOrDefault(r => r.MouseRightDrag(r, DragEvent.Begin, x, y));
        }

        public void MouseRightUp(int x, int y)
        {
            if (Active != null)
            {
                if (_mouseRightDown != null)
                {
                    _mouseRightDown.MouseRightDrag(Active, DragEvent.End, x, y);
                    _mouseRightDown = null;
                }
            }
            if (!IsAnyDown)
            {
                Active = null;
            }
        }

        public void MouseMiddleDown(int x, int y)
        {
            if (Active == null)
            {
                Active = Hover;
            }
            _mouseMiddleDown = Active.ParentPath.FirstOrDefault(r => r.MouseMiddleDrag(r, DragEvent.Begin, x, y));
        }

        public void MouseMiddleUp(int x, int y)
        {
            if (Active != null)
            {
                if (_mouseMiddleDown != null)
                {
                    _mouseMiddleDown.MouseMiddleDrag(Active, DragEvent.End, x, y);
                    _mouseMiddleDown = null;
                }
            }
            if (!IsAnyDown)
            {
                Active = null;
            }
        }

        public void MouseWheel(int delta)
        {
            var processed = (Active ?? Hover).ParentPath.FirstOrDefault(x => x.Wheel(x, delta));
            if (processed != null)
            {
                Updated?.Invoke();
            }
        }

        public void MouseLeftDoubleClicked()
        {
            var processed = (Active ?? Hover).ParentPath.FirstOrDefault(x => x.MouseLeftDoubleClick(x));
            if (processed != null)
            {
                Updated?.Invoke();
            }
        }
    }
}
