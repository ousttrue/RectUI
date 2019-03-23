using System;
using System.Linq;

namespace RectUI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIContext
    {
        RectRegion _mouseLeftDown;
        public bool IsMouseLeftDown;
        public bool IsMouseRightDown;
        public bool IsMouseMiddleDown;
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
                if (_mouseLeftDown!=null)
                {
                    if(_mouseLeftDown.LeftDrag(Active, DragEvent.Drag, x, y))
                    {
                        Updated?.Invoke();
                    }
                }
            }
        }

        public void MouseLeftDown(int x, int y)
        {
            IsMouseLeftDown = true;
            if (Active != null)
            {
            }
            else
            {
                Active = Hover;
            }

            _mouseLeftDown = Active.ParentPath.FirstOrDefault(r => r.LeftDrag(r, DragEvent.Begin, x, y));
        }

        public void MouseLeftUp(int x, int y)
        {
            IsMouseLeftDown = false;
            if (Active != null)
            {
                if (Active == Hover)
                {
                    Active.LeftClick(Active);
                }

                if(_mouseLeftDown != null){
                    _mouseLeftDown.LeftDrag(Active, DragEvent.End, x, y);
                    _mouseLeftDown = null;
                }

                if (!IsAnyDown)
                {
                    Active = null;
                }
            }
        }

        public void MouseRightDown(int x, int y)
        {
            IsMouseRightDown = true;
            if (Active != null)
            {
            }
            else
            {
                Active = Hover;
            }
        }

        public void MouseRightUp(int x, int y)
        {
            IsMouseRightDown = false;
            if (Active != null)
            {
                if (!IsAnyDown)
                {
                    Active = null;
                }
            }
        }

        public void MouseMiddleDown(int x, int y)
        {
            IsMouseMiddleDown = true;
            if (Active != null)
            {
            }
            else
            {
                Active = Hover;
            }
        }

        public void MouseMiddleUp(int x, int y)
        {
            IsMouseMiddleDown = false;
            if (Active != null)
            {
                if (!IsAnyDown)
                {
                    Active = null;
                }
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
    }
}
