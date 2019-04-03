using RectUI.Graphics;
using System;


namespace RectUI.Widgets
{
    public interface IListSource<T>
    {
        int Count { get; }
        T this[int index] { get; }
        event Action Updated;

        ListItemRegion<T> CreateItem();
    }

    public class ListItemRegion<T> : RectRegion
    {
        public int? SourceIndex
        {
            get;
            set;
        }

        ListRegion<T> ParentList => Parent as ListRegion<T>;

        public T Content
        {
            get
            {
                if (SourceIndex.HasValue)
                {
                    return ParentList.Source[SourceIndex.Value];
                }
                else
                {
                    return default(T);
                }
            }
        }

        public bool IsSelected => SourceIndex == ParentList.SelectedSourceIndex;

        public ListItemRegion()
        {
            NormalColor = ColorKeys.ListItemNormal;
            HoverColor = ColorKeys.ListItemHover;
            ActiveColor = ColorKeys.ListItemActive;
        }

        protected virtual void GetIconCommands(IDrawProcessor rpc, bool isActive, bool isHover)
        {
            if (!SourceIndex.HasValue)
            {
                return;
            }
        }

        protected virtual void GetTextCommands(IDrawProcessor rpc, bool isActive, bool isHover)
        {
            if (!SourceIndex.HasValue)
            {
                return;
            }

            rpc.Text(ID, Rect.ToSharpDX(),
                Content.ToString(),
                GetTextColor(isActive, isHover),
                new TextInfo
                {
                    Font = Style.GetFont(FontSize),
                    Alignment = new TextAlignment
                    {
                        HorizontalAlignment = TextHorizontalAlignment.Left,
                        VerticalAlignment = TextVerticalAlignment.Center,
                    }
                });
        }
        public override void GetDrawCommands(IDrawProcessor rpc, bool isActive, bool isHover)
        {
            if (!SourceIndex.HasValue)
            {
                return;
            }

            {
                // Rect
                var rect = Rect.ToSharpDX();
                rect.X += 16;
                rect.Width -= 16;
                rpc.Rectangle(ID, Rect.ToSharpDX(),
                    GetFillColor(isActive || IsSelected, isHover),
                    GetBorderColor(isActive || IsSelected, isHover)
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

    public interface ISingleSelector<T>
    {
        event Action SelectionChanged;
        int? SelectedSourceIndex { get; }
        T Selected { get; }
    }

    public class ListRegion<T> : RectRegion, ISingleSelector<T>
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
        public IListSource<T> Source => m_source;

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
                m_scrollY = 0;
                SelectedSourceIndex = null;
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
                Layout();
            };

            ItemLeftClicked += (i, content) =>
            {
                // Todo: select
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

        public event Action SelectionChanged;

        int? m_selected;
        public int? SelectedSourceIndex
        {
            get { return m_selected; }
            private set
            {
                if (m_selected == value)
                {
                    return;
                }
                m_selected = value;
                SelectionChanged?.Invoke();
            }
        }

        public T Selected
        {
            get
            {
                if (SelectedSourceIndex.HasValue)
                {
                    return m_source[SelectedSourceIndex.Value];
                }
                else
                {
                    return default(T);
                }
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
                    r.SourceIndex = null;
                }
                else
                {
                    r.SourceIndex = index;
                }

                y += ItemHeight;
            }

            Invalidate();
        }

        public event Action<int, T> ItemLeftClicked;
        private void R_LeftClicked(ListItemRegion<T> r)
        {
            var index = Children.IndexOf(r);
            var first = ScrollY / ItemHeight;
            SelectedSourceIndex = r.SourceIndex;
            ItemLeftClicked?.Invoke(first+index, r.Content);
        }

        public event Action<int, T> ItemLeftDoubleClicked;
        private void R_LeftDoubleClicked(ListItemRegion<T> r)
        {
            var index = Children.IndexOf(r);
            var first = ScrollY / ItemHeight;
            SelectedSourceIndex = r.SourceIndex;
            ItemLeftDoubleClicked?.Invoke(first + index, r.Content);
        }
    }
}
