using System;
using System.Collections.Generic;
using RectUI.Graphics;
using SharpDX;

namespace RectUI
{
    public delegate IEnumerable<DrawCommand> GetDrawCommandsFunc(UIContext uiContext, RectRegion r);

    public enum DragEvent
    {
        Begin,
        Drag,
        End,
    }

    /// <summary>
    /// RectRegion + IRectDrawer => Widget
    /// </summary>
    public class RectRegion
    {
        public virtual Rect Rect
        {
            get;
            set;
        }

        public virtual IEnumerable<RectRegion> Traverse()
        {
            yield return this;
        }

        Style m_style = new Style();
        public Style Style
        {
            get { return m_style; }
            set { m_style = value; }
        }

        public Color4 GetFillColor(UIContext uiContext)
        {
            if(uiContext.Active == this)
            {
                return Style.GetColor(StyleColorKey.FillActive);
            }
            else if(uiContext.Hover == this)
            {
                return Style.GetColor(StyleColorKey.FillHover);
            }
            else
            {
                return Style.GetColor(StyleColorKey.Fill);
            }
        }

        public Color4 GetBorderColor(UIContext uiContext)
        {
            if (uiContext.Active == this)
            {
                return Style.GetColor(StyleColorKey.BorderActive);
            }
            else if (uiContext.Hover == this)
            {
                return Style.GetColor(StyleColorKey.BorderHover);
            }
            else
            {
                return Style.GetColor(StyleColorKey.Border);
            }
        }

        public Color4 GetTextColor(UIContext uiContext)
        {
            if (uiContext.Active == this)
            {
                return Style.GetColor(StyleColorKey.TextActive);
            }
            else if (uiContext.Hover == this)
            {
                return Style.GetColor(StyleColorKey.TextHover);
            }
            else
            {
                return Style.GetColor(StyleColorKey.Text);
            }
        }

        public GetDrawCommandsFunc OnGetDrawCommands = (uiContext, r) =>
        {
            return DrawCommandFactory.DrawRectCommands(r.Rect.ToSharpDX(),
                r.GetFillColor(uiContext),
                r.GetBorderColor(uiContext));
        };

        public RectRegion Parent
        {
            get;
            set;
        }

        public IEnumerable<RectRegion> ParentPath
        {
            get
            {
                for(var x = this; x!=null; x=x.Parent)
                {
                    yield return x;
                }
            }
        }

        public IEnumerable<DrawCommand> GetDrawCommands(UIContext uiContext)
        {
            if (OnGetDrawCommands != null)
            {
                foreach (var c in OnGetDrawCommands(uiContext, this))
                {
                    yield return c;
                }
            }
        }

        public virtual RectRegion MouseMove(int x, int y)
        {
            if (Rect.Contains(x, y))
            {
                return this;
            }
            else
            {
                return null;
            }
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
    }

    public class ContentRegion<T> : RectRegion
    {
        public T Content
        {
            get;
            set;
        }
    }
}
