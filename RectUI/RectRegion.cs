using System.Collections.Generic;


namespace RectUI
{
    public class RectRegion
    {
        public virtual Rect Rect
        {
            get;
            set;
        }

        public virtual IEnumerable<RectRegion> Traverse()
        {
            yield return this;
        }

        public RectRegion()
        { }

        public RectRegion(int x, int y, int w, int h)
        {
            Rect = new Rect(x, y, w, h);
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
    }
}
