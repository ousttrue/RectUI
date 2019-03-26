using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RectUI.Graphics;
using SharpDX;


namespace RectUI.Widgets
{
    public delegate IEnumerable<DrawCommand> GetDrawCommandsFunc(UIContext uiContext, RectRegion r);

    public enum DragEvent
    {
        Begin,
        Drag,
        End,
    }

    public struct Anchor
    {
        public int? Left;
        public int? Top;
        public int? Right;
        public int? Bottom;
    }

    public struct ColorKeys
    {
        public StyleColorKey FillColorKey;
        public StyleColorKey BorderColorKey;
        public StyleColorKey TextColorKey;

        public static ColorKeys PanelNormal =>new ColorKeys
        {
            FillColorKey = StyleColorKey.PanelFill,
            BorderColorKey = StyleColorKey.PanelBorder,
            TextColorKey = StyleColorKey.Text,
        };

        public static ColorKeys ButtonNormal => new ColorKeys
        {
            FillColorKey = StyleColorKey.ButtonFill,
            BorderColorKey = StyleColorKey.ButtonBorder,
            TextColorKey = StyleColorKey.Text,
        };

        public static ColorKeys ButtonHover => new ColorKeys
        {
            FillColorKey = StyleColorKey.ButtonFillHover,
            BorderColorKey = StyleColorKey.ButtonBorderHover,
            TextColorKey = StyleColorKey.Text,
        };

        public static ColorKeys ButtonActive => new ColorKeys
        {
            FillColorKey = StyleColorKey.ButtonFillActive,
            BorderColorKey = StyleColorKey.ButtonBorderActive,
            TextColorKey = StyleColorKey.Text,
        };

        public static ColorKeys ListItemNormal => new ColorKeys
        {
            FillColorKey = StyleColorKey.ListItemFill,
            BorderColorKey = StyleColorKey.ListItemBorder,
            TextColorKey = StyleColorKey.Text,
        };

        public static ColorKeys ListItemHover => new ColorKeys
        {
            FillColorKey = StyleColorKey.ListItemFillHover,
            BorderColorKey = StyleColorKey.ListItemBorderHover,
            TextColorKey = StyleColorKey.Text,
        };

        public static ColorKeys ListItemActive => new ColorKeys
        {
            FillColorKey = StyleColorKey.ListItemFillActive,
            BorderColorKey = StyleColorKey.ListItemBorderActive,
            TextColorKey = StyleColorKey.Text,
        };

    }

    /// <summary>
    /// RectRegion + IRectDrawer => Widget
    /// </summary>
    public class RectRegion : IEnumerable<RectRegion>, IDisposable
    {
        public object Content
        {
            get;
            set;
        }

        #region IEnumerable<RectRegion>
        public virtual void Dispose()
        {
            foreach (var child in m_children)
            {
                child.Dispose();
            }
        }

        List<RectRegion> m_children;

        protected List<RectRegion> Children
        {
            get
            {
                if (m_children == null)
                {
                    m_children = new List<RectRegion>();
                }
                return m_children;
            }
        }

        public IEnumerator<RectRegion> GetEnumerator()
        {
            if (m_children == null)
            {
                return Enumerable.Empty<RectRegion>().GetEnumerator();
            }
            return m_children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<RectRegion> Traverse()
        {
            yield return this;

            if (m_children != null)
            {
                foreach (var child in m_children)
                {
                    foreach (var x in child.Traverse())
                    {
                        if (x != null)
                        {
                            yield return x;
                        }
                    }
                }
            }
        }

        public RectRegion Parent
        {
            get;
            set;
        }

        public IEnumerable<RectRegion> ParentPath
        {
            get
            {
                for (var x = this; x != null; x = x.Parent)
                {
                    yield return x;
                }
            }
        }
        #endregion

        public virtual Rect Rect
        {
            get;
            set;
        }

        public Anchor Anchor
        {
            get;
            set;
        }

        #region Style & DrawCommands
        Style m_style = new Style();
        public Style Style
        {
            get { return m_style; }
            set { m_style = value; }
        }

        protected ColorKeys NormalColor = ColorKeys.PanelNormal;
        protected ColorKeys? HoverColor;
        protected ColorKeys? ActiveColor;

        protected Color4? GetFillColor(bool isActive, bool isHover)
        {
            if(isActive && ActiveColor.HasValue)
            {
                var color = Style.GetColor(ActiveColor.Value.FillColorKey);
                if (color.HasValue)
                {
                    return color;
                }
            }

            if((isActive||isHover) && HoverColor.HasValue){
                var color = Style.GetColor(HoverColor.Value.FillColorKey);
                if (color.HasValue)
                {
                    return color;
                }
            }

            return Style.GetColor(NormalColor.FillColorKey);
        }
        protected Color4? GetBorderColor(bool isActive, bool isHover)
        {
            if (isActive && ActiveColor.HasValue)
            {
                var color = Style.GetColor(ActiveColor.Value.BorderColorKey);
                if (color.HasValue)
                {
                    return color;
                }
            }

            if ((isActive || isHover) && HoverColor.HasValue)
            {
                var color = Style.GetColor(HoverColor.Value.BorderColorKey);
                if (color.HasValue)
                {
                    return color;
                }
            }

            return Style.GetColor(NormalColor.BorderColorKey);
        }
        protected Color4? GetTextColor(bool isActive, bool isHover)
        {
            if (isActive && ActiveColor.HasValue)
            {
                var color = Style.GetColor(ActiveColor.Value.TextColorKey);
                if (color.HasValue)
                {
                    return color;
                }
            }

            if ((isActive || isHover) && HoverColor.HasValue)
            {
                var color = Style.GetColor(HoverColor.Value.TextColorKey);
                if (color.HasValue)
                {
                    return color;
                }
            }

            return Style.GetColor(NormalColor.TextColorKey);
        }

        public virtual IEnumerable<DrawCommand> GetDrawCommands(bool isActive, bool isHover)
        {
            return DrawCommandFactory.DrawRectCommands(Rect.ToSharpDX(),
                GetFillColor(isActive, isHover),
                GetBorderColor(isActive, isHover));
        }
        #endregion

        #region MouseEvents
        public RectRegion MouseMove(int x, int y)
        {
            // children
            if (m_children != null)
            {
                foreach (var r in Enumerable.Reverse(m_children))
                {
                    var hover = r.MouseMove(x, y);
                    if (hover != null)
                    {
                        return hover;
                    }
                }
            }

            // this
            if (Rect.Contains(x, y))
            {
                return this;
            }

            // else
            return null;
        }

        public event Action<RectRegion> LeftClicked;
        public void LeftClick(RectRegion sender)
        {
            LeftClicked?.Invoke(sender);
        }

        public event Action<RectRegion, DragEvent, int, int> LeftDragged;
        public bool LeftDrag(RectRegion sender, DragEvent dragEvent, int x, int y)
        {
            if (LeftDragged == null) return false;
            LeftDragged(sender, dragEvent, x, y);
            return true;
        }

        public event Action<RectRegion, int> OnWheel;
        public bool Wheel(RectRegion sender, int delta)
        {
            if (OnWheel == null) return false;
            OnWheel(sender, delta);
            return true;
        }
        #endregion
    }

    public class PanelRegion: RectRegion
    {
        public void Add(RectRegion child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public override Rect Rect
        {
            get { return base.Rect; }
            set
            {
                base.Rect = value;

                Layout();
            }
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
            else if(tail.HasValue)
            {
                return (total - tail.Value - value, value);
            }
            else
            {
                return (pos, value);
            }
        }

        void Layout()
        {
            foreach(var child in Children)
            {
                var (x, w) = GetPosLen(Rect.Width, child.Rect.X, child.Rect.Width, child.Anchor.Left, child.Anchor.Right);
                var (y, h) = GetPosLen(Rect.Height, child.Rect.Y, child.Rect.Height, child.Anchor.Top, child.Anchor.Bottom);
                child.Rect = new Rect(x, y, w, h);
            }
        }
    }
}
