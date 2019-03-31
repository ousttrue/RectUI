using System.Collections.Generic;

namespace RectUI.Widgets
{
    public struct Anchor
    {
        public int? Left;
        public int? Top;
        public int? Right;
        public int? Bottom;
    }

    public class PanelRegion : RectRegion
    {
        List<Anchor> m_anchors = new List<Anchor>();
        public void Add(Anchor anchor, RectRegion child)
        {
            child.Parent = this;
            Children.Add(child);
            m_anchors.Add(anchor);
        }

        (int, int) GetPosLen(int total, int pos, int value, int? head, int? tail)
        {
            if (head.HasValue && tail.HasValue)
            {
                return (head.Value, total - head.Value - tail.Value);
            }
            else if (head.HasValue)
            {
                return (head.Value, value);
            }
            else if (tail.HasValue)
            {
                return (total - tail.Value - value, value);
            }
            else
            {
                return (pos, value);
            }
        }

        protected override void Layout()
        {
            for (int i = 0; i < Children.Count; ++i)
            {
                var child = Children[i];
                var anchor = m_anchors[i];
                var (x, w) = GetPosLen(Rect.Width, child.Rect.X, child.Rect.Width, anchor.Left, anchor.Right);
                var (y, h) = GetPosLen(Rect.Height, child.Rect.Y, child.Rect.Height, anchor.Top, anchor.Bottom);
                child.Rect = new Rect(x, y, w, h);
            }
        }
    }
}
