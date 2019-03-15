using SharpDX;
using System.Collections.Generic;


namespace RectUI
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
        public Color4? FillColorFocus;
        public Color4? FillColorHover;
        public Color4? FillColorActive;
        public Color4 BorderColor;
        public Color4? BorderColorFocus;
        public Color4? BorderColorHover;
        public Color4? BorderColorActive;

        public Color4 GetBorderColor(DrawInfo d)
        {
            if(d.IsActive)
            {
                return BorderColorActive.HasValue ? BorderColorActive.Value : BorderColor;
            }
            else if (d.IsHover)
            {
                return BorderColorHover.HasValue ? BorderColorHover.Value : BorderColor;
            }
            return BorderColor;
        }

        public Color4 GetFillColor(DrawInfo d)
        {
            if (d.IsActive)
            {
                return FillColorActive.HasValue ? FillColorActive.Value : FillColor;
            }
            else if (d.IsHover)
            {
                return FillColorHover.HasValue ? FillColorHover.Value : FillColor;
            }
            return FillColor;
        }
    }

    public class DrawInfo
    {
        public Rect Rect;
        public bool HasFocus;
        public bool IsHover;
        public bool MouseLeftDown;
        public bool MouseRightDown;
        public bool MouseMiddleDown;
        public bool IsActive
        {
            get
            {
                return MouseLeftDown || MouseRightDown || MouseMiddleDown;
            }
        }
        public int MouseX;
        public int MouseY;
        public bool IsMouseInclude
        {
            get
            {
                return Rect.Include(MouseX, MouseY);
            }
        }

        public void MouseMove(int x, int y)
        {
            IsHover = Rect.Include(x, y);
            MouseX = x;
            MouseY = y;
        }
    }

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

    /// <summary>
    /// 水平に領域を分割する
    /// </summary>
    public class HorizontalSplitter : RectRegion
    {
        const int _splitterWidth = 8;
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

        public override void MouseLeftDown()
        {
            foreach (var child in _regions)
            {
                child.MouseLeftDown();
            }
        }
        public override void MouseLeftUp()
        {
            foreach (var child in _regions)
            {
                child.MouseLeftUp();
            }
        }
        public override void MouseRightDown()
        {
            foreach (var child in _regions)
            {
                child.MouseRightDown();
            }
        }
        public override void MouseRightUp()
        {
            foreach (var child in _regions)
            {
                child.MouseRightUp();
            }
        }
        public override void MouseMiddleDown()
        {
            foreach (var child in _regions)
            {
                child.MouseMiddleDown();
            }
        }
        public override void MouseMiddleUp()
        {
            foreach (var child in _regions)
            {
                child.MouseMiddleUp();
            }
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
