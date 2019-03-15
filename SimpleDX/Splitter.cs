using SharpDX;
using System.Collections.Generic;


namespace SimpleDX
{
    public struct Rect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public bool Include(int x, int y)
        {
            if (x < X) return false;
            if (x > X + Width) return false;
            if (y < Y) return false;
            if (y > Y + Height) return false;
            return true;
        }

        public Rect(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
    }

    public class Thema
    {
        public Color4 FillColor;
        public Color4 FillColorFocus;
        public Color4 FillColorHover;
        public Color4 FillColorActive;
        public Color4 BorderColor;
        public Color4 BorderColorFocus;
        public Color4 BorderColorHover;
        public Color4 BorderColorActive;

        public Color4 GetBorderColor(DrawInfo d)
        {
            return d.IsHover ? BorderColorHover : BorderColorHover;
        }

        public Color4 GetFillColor(DrawInfo d)
        {
            return d.IsHover ? FillColorHover : FillColor;
        }
    }

    public class DrawInfo
    {
        public Rect Rect;
        public bool HasFocus;
        public bool IsHover;
        public bool IsActive; // Drag

        public void MouseMove(int x, int y)
        {
            IsHover = Rect.Include(x, y);
        }
    }

    class RectRegion
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
        int _mouseX;
        int _mouseY;

        public virtual void MouseMove(int parentX, int parentY)
        {
            _drawInfo.MouseMove(parentX, parentY);

            _mouseX = parentX - Rect.X;
            _mouseY = parentY - Rect.Y; ;
        }
        #endregion
    }

    /// <summary>
    /// 水平に領域を分割する
    /// </summary>
    class HorizontalSplitter : RectRegion
    {
        const int _splitterWidth = 5;
        List<RectRegion> _splitters = new List<RectRegion>();

        List<RectRegion> _regions = new List<RectRegion>();
        public void Add(RectRegion region)
        {
            if (_regions.Count > 0)
            {
                // add _splitter
                _splitters.Add(new RectRegion());
            }
            _regions.Add(region);

            Layout();
        }

        public override Rect Rect
        {
            get { return base.Rect; }
            set
            {
                base.Rect = value;
                Layout();
            }
        }

        void Layout()
        {
            if (_regions.Count == 0)
            {
                return;
            }

            if (_regions.Count == 1)
            {
                _regions[0].Rect = Rect;
                return;
            }

            var w = (Rect.Width - _splitterWidth * _splitters.Count) / _regions.Count;
            int x = 0;
            int y = 0;
            for (int i = 0; i < _regions.Count; ++i)
            {
                if (i > 0)
                {
                    var splitter = _splitters[i - 1];
                    splitter.Rect = new Rect(x, y, w, Rect.Height);
                    x += _splitterWidth;
                }

                var child = _regions[i];
                child.Rect = new Rect(x, y, w, Rect.Height);
                x += w;
            }
        }

        public override IEnumerable<DrawInfo> Traverse()
        {
            foreach (var child in _regions)
            {
                foreach (var d in child.Traverse())
                {
                    yield return d;
                }
            }
        }

        public HorizontalSplitter(int w, int h) : base(0, 0, w, h)
        {
        }

        public override void MouseMove(int parentX, int parentY)
        {
            var x = parentX - Rect.X;
            var y = parentY - Rect.Y;

            foreach (var child in _regions)
            {
                child.MouseMove(x, y);
            }
        }
    }
}
