namespace RectUI.Widgets
{
    public class PanelRegion : RectRegion
    {
        public void Add(RectRegion child)
        {
            child.Parent = this;
            Children.Add(child);
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
            foreach (var child in Children)
            {
                var (x, w) = GetPosLen(Rect.Width, child.Rect.X, child.Rect.Width, child.Anchor.Left, child.Anchor.Right);
                var (y, h) = GetPosLen(Rect.Height, child.Rect.Y, child.Rect.Height, child.Anchor.Top, child.Anchor.Bottom);
                child.Rect = new Rect(x, y, w, h);
            }
        }
    }
}
