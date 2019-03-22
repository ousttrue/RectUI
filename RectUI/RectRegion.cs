using System;
using System.Collections.Generic;


namespace RectUI
{
    /// <summary>
    /// RectRegion + IRectDrawer => Widget
    /// </summary>
    public class RectRegion
    {
        public virtual Rect Rect
        {
            get;
            set;
        }

        public IRectDrawer Drawer
        {
            get;
            set;
        }

        public virtual IEnumerable<RectRegion> Traverse()
        {
            yield return this;
        }

        public virtual RectRegion MouseMove(int x, int y)
        {
            if (Rect.Include(x, y))
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        public event Action<RectRegion> LeftClicked;
        public void LeftClick(RectRegion sender)
        {
            LeftClicked?.Invoke(sender);
        }
    }
}
