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
        public RectRegion Hover
        {
            get;
            set;
        }

        /// <summary>
        /// When Drag any
        /// </summary>
        public RectRegion Active
        {
            get;
            set;
        }

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
