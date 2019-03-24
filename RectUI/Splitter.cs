using SharpDX;
using System.Collections.Generic;


namespace RectUI
{
    /// <summary>
    /// 水平に領域を分割する
    /// </summary>
    public class HorizontalSplitter : RectRegion
    {
        public RectRegion Left
        {
            get { return m_children[1]; }
            set
            {
                if (m_children[1] == value) return;
                m_children[1] = value;
                Layout();
            }
        }

        public RectRegion Right
        {
            get { return m_children[2]; }
            set
            {
                if (m_children[2] == value) return;
                m_children[2] = value;
                Layout();
            }
        }

        const int _splitterWidth = 8;

        RectRegion Splitter
        {
            get { return m_children[0]; }
        }

        public HorizontalSplitter()
        {
            m_children.Add(new RectRegion()); // splitter
            m_children.Add(null); // left
            m_children.Add(null); // right
            Splitter.LeftDragged += Splitter_LeftDragged;
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
    }
}
