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

        event Action<T> Entered;
        void Enter(int index);

        ListItemRegion<T> CreateItem();
    }

    public abstract class ListSource<T> : IListSource<T>, IEnumerable<T>
    {
        protected List<T> m_items = new List<T>();
        public int Count => m_items.Count;

        public T this[int index] { get { return m_items[index]; } }

        public event Action Updated;
        protected void RaiseUpdate()
        {
            Updated?.Invoke();
        }

        public event Action<T> Entered;
        public void Enter(int index)
        {
            if (index >= 0 && index < Count)
            {
                Entered?.Invoke(this[index]);
            }
        }

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

        public abstract ListItemRegion<T> CreateItem();
    }

    public abstract class ListItemRegion<T> : RectRegion
    {
        public T Content
        {
            get;
            set;
        }

        public ListItemRegion()
        {
            NormalColor = ColorKeys.ListItemNormal;
            HoverColor = ColorKeys.ListItemHover;
            ActiveColor = ColorKeys.ListItemActive;
        }

        protected abstract void GetIconCommands(IDrawRPC rpc, bool isActive, bool isHover);
        protected abstract void GetTextCommands(IDrawRPC rpc, bool isActive, bool isHover);
        public override void GetDrawCommands(IDrawRPC rpc, bool isActive, bool isHover)
        {
            if (Content == null)
            {
                return;
            }

            {
                // Rect
                var rect = Rect.ToSharpDX();
                rect.X += 16;
                rect.Width -= 16;
                rpc.Rectangle(ID, Rect.ToSharpDX(),
                    GetFillColor(isActive, isHover),
                    GetBorderColor(isActive, isHover)
                );
            }

            GetIconCommands(rpc, isActive, isHover);

            GetTextCommands(rpc, isActive, isHover);
        }

        protected Padding m_padding = new Padding
        {
            Left = 21,
            Top = 3,
            Right = 5,
            Bottom = 2,
        };
    }

    public class ListRegion<T> : RectRegion
    {
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

        int MaxScrollY
        {
            get
            {
                var len = m_source.Count * m_itemHeight - Rect.Height;
                if (len < 0)
                {
                    len = 0;
                }
                return len;
            }
        }

        public ListRegion(IListSource<T> source)
        {
            m_source = source;
            source.Updated += () =>
            {
                Layout();
                m_scrollY = 0;
                Invalidate();
            };

            OnWheel += (_, delta) =>
            {
                if (delta < 0)
                {
                    ScrollY += ItemHeight * 2;
                }
                else
                {
                    ScrollY -= ItemHeight * 2;
                }
                if (ScrollY < 0)
                {
                    ScrollY = 0;
                }
                else if (ScrollY > MaxScrollY)
                {
                    ScrollY = MaxScrollY;
                }
            };

            ItemLeftClicked += (i, content) =>
            {
                // Todo: select
            };
            ItemLeftDoubleClicked += (i, content)=>
            {
                source.Enter(i);
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

        protected override void Layout()
        {
            var count = Rect.Height / ItemHeight + 1;

            var index = m_scrollY / ItemHeight;

            var y = Rect.Y + (index * ItemHeight - m_scrollY);
            int i = 0;
            for (; i < count; ++i, ++index)
            {
                ListItemRegion<T> r = null;
                if (i < Children.Count)
                {
                    r = Children[i] as ListItemRegion<T>;
                }
                else
                {
                    r = m_source.CreateItem();
                    r.Parent = this;
                    r.MouseLeftClicked += x => R_LeftClicked(x as ListItemRegion<T>);
                    r.MouseLeftDoubleClicked += x => R_LeftDoubleClicked(x as ListItemRegion<T>);
                    Children.Add(r);
                }

                r.Rect = new Rect(Rect.X, y, Rect.Width, ItemHeight);
                if (index < 0 || index >= m_source.Count)
                {
                    r.Content = default(T);
                }
                else
                {
                    r.Content = m_source[index];
                }

                y += ItemHeight;
            }
        }

        public event Action<int, T> ItemLeftClicked;
        private void R_LeftClicked(ListItemRegion<T> r)
        {
            var index = Children.IndexOf(r);
            var first = ScrollY / ItemHeight;
            ItemLeftClicked?.Invoke(first+index, r.Content);
        }

        public event Action<int, T> ItemLeftDoubleClicked;
        private void R_LeftDoubleClicked(ListItemRegion<T> r)
        {
            var index = Children.IndexOf(r);
            var first = ScrollY / ItemHeight;
            ItemLeftDoubleClicked?.Invoke(first + index, r.Content);
        }
    }
}
