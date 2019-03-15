using System.Collections.Generic;


namespace SimpleDX
{
    struct Rect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Rect(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
    }

    struct DrawInfo
    {
        public Rect Rect;
        public bool Focus;
        public bool Hover;
        public bool Active; // Drag
    }

    class RectRegion
    {
        public virtual IEnumerable<DrawInfo> Traverse()
        {
            yield return new DrawInfo
            {
                Rect = _rect,
            };
        }

        #region Rect
        protected Rect _rect;

        public RectRegion()
        { }

        public RectRegion(int x, int y, int w, int h)
        {
            SetRect(x, y, w, h);
        }

        public void SetRect(int x, int y, int w, int h)
        {
            _rect = new Rect(x, y, w, h);
        }
        #endregion

        #region Mouse
        int _mouseX;
        int _mouseY;

        public virtual void MouseMove(int parentX, int parentY)
        {
            var x = parentX - _rect.X;
            var y = parentY - _rect.Y;

            _mouseX = x;
            _mouseY = y;
        }
        #endregion
    }

    /// <summary>
    /// 水平に領域を分割する
    /// </summary>
    class HorizontalSplitter : RectRegion
    {
        List<RectRegion> _children = new List<RectRegion>();
        public void Add(RectRegion region)
        {
            var div = _children.Count + 1;

            var w = _rect.Width / div;

            _children.Add(region);

            int x = 0;
            int y = 0;
            foreach (var child in _children)
            {
                child.SetRect(x, y, w, _rect.Height);
                x += w;
            }
        }

        public HorizontalSplitter(int w, int h) : base(0, 0, w, h)
        {
        }

        public override IEnumerable<DrawInfo> Traverse()
        {
            foreach(var child in _children)
            {
                foreach(var d in child.Traverse())
                {
                    yield return d;
                }
            }
        }

        public override void MouseMove(int parentX, int parentY)
        {
            var x = parentX - _rect.X;
            var y = parentY - _rect.Y;

            foreach (var child in _children)
            {
                child.MouseMove(x, y);
            }
        }
    }
}
