namespace RectUI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIContext
    {
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
            if (Active != null)
            {
                Active.MouseLeftDown();
            }
            else
            {
                root.MouseLeftDown();
            }
        }

        public void MouseLeftUp(RectRegion root)
        {
            if (Active != null)
            {
                Active.MouseLeftUp();
            }
            else
            {
                root.MouseLeftUp();
            }
        }

        public void MouseRightDown(RectRegion root)
        {
            if (Active != null)
            {
                Active.MouseRightDown();
            }
            else
            {
                root.MouseRightDown();
            }
        }

        public void MouseRightUp(RectRegion root)
        {
            if (Active != null)
            {
                Active.MouseRightUp();
            }
            else
            {
                root.MouseRightUp();
            }
        }

        public void MouseMiddleDown(RectRegion root)
        {
            if (Active != null)
            {
                Active.MouseMiddleDown();
            }
            else
            {
                root.MouseMiddleDown();
            }
        }

        public void MouseMiddleUp(RectRegion root)
        {
            if (Active != null)
            {
                Active.MouseMiddleUp();
            }
            else
            {
                root.MouseMiddleUp();
            }
        }

        public void MouseMove(RectRegion root, int x, int y)
        {
            if (Active != null)
            {
                Active.MouseMove(x, y);
            }
            else
            {
                root.MouseMove(x, y);
            }
        }
    }
}
