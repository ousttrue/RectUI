using System.Collections.Generic;


namespace RectUI
{
    public class RectRegion
    {
        public virtual IEnumerable<DrawInfo> Traverse()
        {
            yield return _drawInfo;
        }

        DrawInfo _drawInfo = new DrawInfo();

        #region Rect
        public virtual Rect Rect
        {
            get { return _drawInfo.Rect; }
            set { _drawInfo.Rect = value; }
        }

        public RectRegion()
        { }

        public RectRegion(int x, int y, int w, int h)
        {
            Rect = new Rect(x, y, w, h);
        }
        #endregion

        #region Mouse
        public virtual void MouseLeftDown()
        {
            if (_drawInfo.IsMouseInclude)
            {
                _drawInfo.MouseLeftDown = true;
            }
        }
        public virtual void MouseLeftUp()
        {
            _drawInfo.MouseLeftDown = false;
        }
        public virtual void MouseRightDown()
        {
            _drawInfo.MouseRightDown = true;
        }
        public virtual void MouseRightUp()
        {
            _drawInfo.MouseRightDown = false;
        }
        public virtual void MouseMiddleDown()
        {
            _drawInfo.MouseMiddleDown = true;
        }
        public virtual void MouseMiddleUp()
        {
            _drawInfo.MouseMiddleDown = false;
        }

        public virtual void MouseMove(int x, int y)
        {
            _drawInfo.MouseMove(x, y);
        }
        #endregion
    }
}
