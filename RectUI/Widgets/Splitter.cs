using SharpDX;
using System.Collections.Generic;


namespace RectUI.Widgets
{
    /// <summary>
    /// 水平に領域を分割する
    /// </summary>
    public class HorizontalSplitter : RectRegion
    {
        public RectRegion Left
        {
            get { return Children[0]; }
            set
            {
                if (Children[0] == value) return;
                Children[0] = value;
                Layout();
            }
        }

        public RectRegion Right
        {
            get { return Children[1]; }
            set
            {
                if (Children[1] == value) return;
                Children[1] = value;
                Layout();
            }
        }

        RectRegion Splitter
        {
            get { return Children[2]; }
        }

        const int _splitterWidth = 8;

        public HorizontalSplitter()
        {
            Children.Add(null); // left
            Children.Add(null); // right
            Children.Add(new ButtonRegion()); // splitter
            Splitter.MouseLeftDragged += Splitter_LeftDragged;
        }

        private void Splitter_LeftDragged(RectRegion r, DragEvent dragEvent, int x, int y)
        {
            switch (dragEvent)
            {
                case DragEvent.Begin:
                    break;

                case DragEvent.Drag:
                    Splitter.Rect = new Rect(x, Splitter.Rect.Y, 
                        Splitter.Rect.Width, Splitter.Rect.Height);
                    Layout();
                    break;

                case DragEvent.End:
                    break;
            }

        }

        protected override void Layout()
        {
            if (Splitter.Rect.Height == 0)
            {
                Splitter.Rect = new Rect(Rect.X + Rect.Width / 2, Rect.Y, _splitterWidth, Rect.Height);
            }
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
    }
}
