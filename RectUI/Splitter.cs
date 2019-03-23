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
                _splitters.Add(new RectRegion { Parent = this });
            }

            region.Parent = this;
            _regions.Add(region);

            Layout();
        }

        public override IEnumerable<RectRegion> Traverse()
        {
            foreach (var r in _regions)
            {
                foreach(var x in r.Traverse())
                {
                    yield return x;
                }
            }

            foreach(var s in _splitters)
            {
                foreach(var x in s.Traverse())
                {
                    yield return x;
                }
            }
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
                    splitter.Rect = new Rect(x, y, _splitterWidth, Rect.Height);
                    x += _splitterWidth;
                }

                var child = _regions[i];
                child.Rect = new Rect(x, y, w, Rect.Height);
                x += w;
            }
        }

        public override RectRegion MouseMove(int parentX, int parentY)
        {
            var x = parentX - Rect.X;
            var y = parentY - Rect.Y;

            foreach(var s in _splitters)
            {
                var hover = s.MouseMove(x, y);
                if (hover != null)
                {
                    return hover;
                }
            }

            foreach (var child in _regions)
            {
                var hover = child.MouseMove(x, y);
                if (hover != null)
                {
                    return hover;
                }
            }

            return null;
        }
    }
}
