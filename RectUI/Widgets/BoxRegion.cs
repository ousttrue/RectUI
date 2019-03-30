﻿namespace RectUI.Widgets
{
    public abstract class BoxRegion : RectRegion
    {
        protected BoxRegion(Rect rect)
        {
            Rect = rect;
        }

        public void Add(RectRegion child)
        {
            child.BoxItem = BoxItem.Fixed;
            child.Parent = this;
            Children.Add(child);
        }

        public void Add(BoxItem boxItem, RectRegion child)
        {
            child.BoxItem = boxItem;
            child.Parent = this;
            Children.Add(child);
        }

        protected abstract int GetLength(Rect r);
        protected abstract Rect CreateRect(ref int pos, int length);

        protected override void Layout()
        {
            // fix が使った残りの領域を得る
            var remain = GetLength(Rect);
            var expandCount = 0;
            foreach (var child in Children)
            {
                switch (child.BoxItem)
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
            foreach (var child in Children)
            {
                switch (child.BoxItem)
                {
                    case BoxItem.Fixed:
                        child.Rect = CreateRect(ref pos, child.Rect.Height);
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
            var rect = new Rect(Rect.X, Rect.Y + pos, Rect.Width, length);
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
            var rect = new Rect(Rect.X + pos, Rect.Y, length, Rect.Height);
            pos += length;
            return rect;
        }

        protected override int GetLength(Rect r)
        {
            return r.Width;
        }
    }
}