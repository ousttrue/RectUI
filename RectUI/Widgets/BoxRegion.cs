using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RectUI.Widgets
{
    /// <summary>
    /// 縦に整列させる
    /// </summary>
    public class VBoxRegion : RectRegion
    {
        public void Add(RectRegion child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        protected override void Layout()
        {
            // fix が使った残りの領域を得る
            var remain = Rect.Height;
            var expandCount = 0;
            foreach (var child in Children)
            {
                switch (child.BoxItem)
                {
                    case BoxItem.Fixed:
                        remain -= child.Rect.Height;
                        break;

                    case BoxItem.Expand:
                        ++expandCount;
                        break;
                }
            }
            var expandSize = remain / expandCount;

            // レイアウト
            var pos = 0;
            foreach(var child in Children)
            {
                switch(child.BoxItem)
                {
                    case BoxItem.Fixed:
                        child.Rect = new Rect(Rect.X, Rect.Y + pos, Rect.Width, child.Rect.Height);
                        pos += child.Rect.Height;
                        break;

                    case BoxItem.Expand:
                        child.Rect = new Rect(Rect.X, Rect.Y + pos, Rect.Width, expandSize);
                        pos += expandSize;
                        break;
                }
            }
        }
    }
}
