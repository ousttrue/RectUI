using System;

namespace RectUI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIContext
    {
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

        public void MouseLeftDown(RectRegion root)
        {
            IsMouseLeftDown = true;
            if (Active != null)
            {
            }
            else
            {
                Active = Hover;
            }
        }

        public void MouseLeftUp(RectRegion root)
        {
            IsMouseLeftDown = false;
            if (Active != null)
            {
                if (Active == Hover)
                {
                    Active.LeftClick(Active);
                }

                if (!IsAnyDown)
                {
                    Active = null;
                }
            }
        }

        public void MouseRightDown(RectRegion root)
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

        public void MouseRightUp(RectRegion root)
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

        public void MouseMiddleDown(RectRegion root)
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

        public void MouseMiddleUp(RectRegion root)
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

        public void MouseMove(RectRegion root, int x, int y)
        {
            MouseX = x;
            MouseY = y;
            Hover = root.MouseMove(MouseX, MouseY);
        }
    }
}
