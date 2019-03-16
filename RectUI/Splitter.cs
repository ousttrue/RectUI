using SharpDX;
using System.Collections.Generic;


namespace RectUI
{
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
