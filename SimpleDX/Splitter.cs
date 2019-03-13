using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SimpleDX
{
    class RectRegion
    {
        int _localX;
        int _localY;
        int _width;
        int _height;

        public virtual void Draw(int x, int y)
        {

        }

        public RectRegion(int x, int y, int w, int h)
        {
            SetRect(x, y, w, h);
        }

        public void SetRect(int x, int y, int w, int h)
        {
            _localX = x;
            _localY = y;
            _width = w;
            _height = h;
        }

        int _mouseX;
        int _mouseY;

        public void MouseMove(int x, int y)
        {
            _mouseX = x;
            _mouseY = y;
        }
    }

    /// <summary>
    /// 水平に領域を分割する
    /// </summary>
    class HorizontalSplitter : RectRegion
    {
        RectRegion _left;
        RectRegion _right;

        public override void Draw(int x, int y)
        {

        }

        public HorizontalSplitter(int w, int h) : base(0, 0, w, h)
        {
            var half = w / 2;
            _left = new RectRegion(0, 0, half, h);
            _right = new RectRegion(half, 0, w - half, h);
        }
    }
}
