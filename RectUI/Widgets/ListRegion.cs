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

    public class ListSource<T> : IListSource<T>, IEnumerable<T>
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

        public virtual ListItemRegion<T> CreateItem()
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

        public virtual IEnumerable<D2DDrawCommand> GetIconCommands()
        {
            yield break;
        }

        public override IEnumerable<IEnumerable<D2DDrawCommand>> GetDrawCommands(bool isActive, bool isHover)
        {
            if (Content == null)
            {
                yield break;
            }

            var rect = Rect.ToSharpDX();
            rect.X += 16;
            rect.Width -= 16;
            yield return D2DDrawCommandFactory.DrawRectCommands(rect,
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
            yield return D2DDrawCommandFactory.DrawTextCommands(Rect.ToSharpDX(), m_padding,
                color, 
                new FontInfo
                {
                    Font = "MS Gothic",
                    Size = Rect.Height-m_padding.Top-m_padding.Bottom,
                },
                new TextInfo
                {
                    Text = Content.ToString(),
                    HorizontalAlignment = TextHorizontalAlignment.Left,
                    VerticalAlignment = TextVerticalAlignment.Center,
                }
                );
        }

        Padding m_padding = new Padding
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
                RectRegion r = null;
                if (i < Children.Count)
                {
                    r = Children[i];
                }
                else
                {
                    r = m_source.CreateItem();
                    r.Parent = this;
                    r.MouseLeftClicked += x => R_LeftClicked(x);
                    r.MouseLeftDoubleClicked += x => R_LeftDoubleClicked(x);
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

        public event Action<int, object> ItemLeftDoubleClicked;
        private void R_LeftDoubleClicked(RectRegion r)
        {
            var index = Children.IndexOf(r);
            var first = ScrollY / ItemHeight;
            ItemLeftDoubleClicked?.Invoke(first + index, r.Content);
        }
    }
}
