using RectUI.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;


namespace RectUI.Widgets
{
    public interface IListSource<T>
    {
        int Count { get; }
        T this[int index] { get; }
        event Action Updated;
        ListItemRegion<T> CreateItem();
    }

    public class ListSource<T> : IListSource<T>, IEnumerable<T>
    {
        List<T> m_items = new List<T>();
        public int Count => m_items.Count;

        public T this[int index] { get { return m_items[index]; } }

        public event Action Updated;

        public IEnumerator<T> GetEnumerator()
        {
            return m_items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T value)
        {
            m_items.Add(value);
        }

        public ListItemRegion<T> CreateItem()
        {
            return new ListItemRegion<T>(this)
            {
            };
        }
    }

    public class ListItemRegion<T> : RectRegion
    {
        IListSource<T> m_source;

        public ListItemRegion(IListSource<T> source)
        {
            m_source = source;

            NormalColor = ColorKeys.ListItemNormal;
            HoverColor = ColorKeys.ListItemHover;
            ActiveColor = ColorKeys.ListItemActive;
        }

        public virtual IEnumerable<DrawCommand> GetIconCommands()
        {
            yield break;
        }

        public override IEnumerable<IEnumerable<DrawCommand>> GetDrawCommands(bool isActive, bool isHover)
        {
            if (Content == null)
            {
                yield break;
            }

            var rect = Rect.ToSharpDX();
            rect.X += 16;
            rect.Width -= 16;
            yield return DrawCommandFactory.DrawRectCommands(rect,
                GetFillColor(isActive, isHover),
                GetBorderColor(isActive, isHover));

            /*
            if (dir.Current.Parent.FullName == r.Content.FullName)
            {
                label = "..";
            }
            */

            yield return GetIconCommands();

            var color = GetTextColor(isActive, isHover);
            yield return DrawCommandFactory.DrawTextCommands(this,
                color, "MS Gothic", Rect.Height,
                21, 3, 5, 2,
                Content.ToString());
        }
    }


    public class ListRegion<T> : RectRegion
    {
        public override Rect Rect
        {
            get => base.Rect;
            set
            {
                base.Rect = value;
                Layout();
            }
        }

        int m_itemHeight = 18;
        public int ItemHeight
        {
            get { return m_itemHeight = 18; }
            set
            {
                if (m_itemHeight == value) return;
                m_itemHeight = value;
                if (m_itemHeight < 1)
                {
                    m_itemHeight = 1;
                }
            }
        }

        IListSource<T> m_source;

        public ListRegion(IListSource<T> source)
        {
            m_source = source;
            source.Updated += () =>
            {
                Layout();
            };
        }

        int m_scrollY = 0;
        public int ScrollY
        {
            get { return m_scrollY; }
            set
            {
                if (m_scrollY == value) return;
                m_scrollY = value;
                Layout();
            }
        }

        void Layout()
        {
            var count = Rect.Height / ItemHeight + 1;

            var index = m_scrollY / ItemHeight;

            var y = Rect.Y + (index * ItemHeight - m_scrollY);
            int i = 0;
            for (; i < count; ++i, ++index)
            {
                RectRegion r = null;
                if (i < Children.Count)
                {
                    r = Children[i];
                }
                else
                {
                    r = m_source.CreateItem();
                    r.Parent = this;
                    r.LeftClicked += x => R_LeftClicked(x);
                    Children.Add(r);
                }

                r.Rect = new Rect(Rect.X, y, Rect.Width, ItemHeight);
                if (index < 0 || index >= m_source.Count)
                {
                    r.Content = null;
                }
                else
                {
                    r.Content = m_source[index];
                }

                y += ItemHeight;
            }
        }

        public event Action<int, object> ItemLeftClicked;
        private void R_LeftClicked(RectRegion r)
        {
            var index = Children.IndexOf(r);
            var first = ScrollY / ItemHeight;
            ItemLeftClicked?.Invoke(first+index, r.Content);
        }
    }
}
