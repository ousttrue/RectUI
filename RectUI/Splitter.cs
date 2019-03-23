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

        RectRegion _splitter = new RectRegion();
        RectRegion Splitter
        {
            get
            {
                return _splitter;
            }
        }

        public HorizontalSplitter()
        {
            //Splitter.
        }

        RectRegion _left;
        public RectRegion Left
        {
            get { return _left; }
            set
            {
                if (_left == value) return;
                _left = value;
                Layout();
            }
        }

        RectRegion _right;
        public RectRegion Right
        {
            get { return _right; }
            set
            {
                if (_right == value) return;
                _right = value;
                Layout();
            }
        }

        public override IEnumerable<RectRegion> Traverse()
        {
            if (Left != null)
            {
                foreach (var x in Left.Traverse())
                {
                    yield return x;
                }
            }
            if (Right != null)
            {
                foreach (var x in Right.Traverse())
                {
                    yield return x;
                }
            }

            {
                foreach(var x in Splitter.Traverse())
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

                if (Splitter.Rect.Height == 0)
                {
                    Splitter.Rect = new Rect(Rect.X + Rect.Width / 2, Rect.Y, _splitterWidth, Rect.Height);
                }

                Layout();
            }
        }

        void Layout()
        {
            Splitter.Rect = new Rect(Splitter.Rect.X, Splitter.Rect.Y, _splitterWidth, Rect.Height);

            if (Left != null)
            {
                Left.Rect = new Rect(Rect.X, Rect.Y,
                    Splitter.Rect.X, Rect.Height);
            }

            if (Right != null)
            {
                Right.Rect = new Rect(Splitter.Rect.X + Splitter.Rect.Width, Rect.Y,
                    Rect.Width - Splitter.Rect.X - Splitter.Rect.Width, Rect.Height);
            }
        }

        public override RectRegion MouseMove(int x, int y)
        {
            {
                var hover = Splitter.MouseMove(x, y);
                if (hover != null)
                {
                    return hover;
                }
            }

            if(Left!=null)
            {
                var hover = Left.MouseMove(x, y);
                if (hover != null)
                {
                    return hover;
                }
            }

            if (Right != null)
            {
                var hover = Right.MouseMove(x, y);
                if (hover != null)
                {
                    return hover;
                }
            }

            return null;
        }
    }
}
