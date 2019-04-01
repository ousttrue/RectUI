using System.Collections.Generic;

namespace RectUI.Widgets
{
    public enum BoxItem
    {
        Fixed,
        Expand,
    }

    public abstract class BoxRegion : RectRegion
    {
        protected BoxRegion(Rect rect)
        {
            Rect = rect;
        }

        readonly List<BoxItem> m_boxItems = new List<BoxItem>();
        public void Add(RectRegion child)
        {
            child.Parent = this;
            Children.Add(child);
            m_boxItems.Add(BoxItem.Fixed);
        }

        public void Add(BoxItem boxItem, RectRegion child)
        {
            child.Parent = this;
            Children.Add(child);
            m_boxItems.Add(boxItem);
        }

        protected abstract int GetLength(Rect r);
        protected abstract Rect CreateRect(ref int pos, int length);

        protected override void Layout()
        {
            // fix が使った残りの領域を得る
            var remain = GetLength(Rect);
            var expandCount = 0;
            for (int i = 0; i < Children.Count; ++i)
            {
                var child = Children[i];
                var boxItem = m_boxItems[i];
                switch (boxItem)
                {
                    case BoxItem.Fixed:
                        remain -= GetLength(child.Rect);
                        break;

                    case BoxItem.Expand:
                        ++expandCount;
                        break;
                }
            }
            var expandSize = 0;
            if (expandCount > 0)
            {
                expandSize = remain / expandCount;
            }

            // レイアウト
            var pos = 0;
            for (int i = 0; i < Children.Count; ++i)
            {
                var child = Children[i];
                var boxItem = m_boxItems[i];
                switch (boxItem)
                {
                    case BoxItem.Fixed:
                        child.Rect = CreateRect(ref pos, GetLength(child.Rect));
                        break;

                    case BoxItem.Expand:
                        child.Rect = CreateRect(ref pos, expandSize);
                        break;
                }
            }
        }
    }

    public class VBoxRegion : BoxRegion
    {
        public VBoxRegion(Rect rect = default(Rect)) : base(rect)
        { }

        protected override Rect CreateRect(ref int pos, int length)
        {
            var rect = new Rect(
                Rect.X, Rect.Y + pos, 
                Rect.Width, length);
            pos += length;
            return rect;
        }

        protected override int GetLength(Rect r)
        {
            return r.Height;
        }
    }

    public class HBoxRegion : BoxRegion
    {
        public HBoxRegion(Rect rect = default(Rect)) : base(rect)
        { }

        protected override Rect CreateRect(ref int pos, int length)
        {
            var rect = new Rect(
                Rect.X + pos, Rect.Y, 
                length, Rect.Height);
            pos += length;
            return rect;
        }

        protected override int GetLength(Rect r)
        {
            return r.Width;
        }
    }
}
