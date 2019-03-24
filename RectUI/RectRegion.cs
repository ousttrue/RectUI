using System;
using System.Collections;
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

        protected List<RectRegion> m_children = new List<RectRegion>();

        public IEnumerator<RectRegion> GetEnumerator()
        {
            return m_children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<RectRegion> Traverse()
        {
            yield return this;

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

        #region Style & DrawCommands
        Style m_style = new Style();
        public Style Style
        {
            get { return m_style; }
            set { m_style = value; }
        }

        public virtual Color4? GetFillColor(UIContext uiContext)
        {
            return Style.GetColor(StyleColorKey.PanelFill);
        }

        public virtual Color4? GetBorderColor(UIContext uIContext)
        {
            return Style.GetColor(StyleColorKey.PanelBorder);
        }

        public virtual Color4? GetTextColor(UIContext uIContext)
        {
            return Style.GetColor(StyleColorKey.Text);
        }

        public GetDrawCommandsFunc OnGetDrawCommands = (uiContext, r) =>
        {
            return DrawCommandFactory.DrawRectCommands(r.Rect.ToSharpDX(),
                r.GetFillColor(uiContext),
                r.GetBorderColor(uiContext));
        };

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
        #endregion

        #region MouseEvents
        public RectRegion MouseMove(int x, int y)
        {
            // children
            foreach (var r in m_children)
            {
                var hover = r.MouseMove(x, y);
                if (hover != null)
                {
                    return hover;
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
            m_children.Add(child);
        }
    }

    public class ButtonRegion : RectRegion
    {
        Action<RectRegion> m_action;

        public ButtonRegion(Action<RectRegion> action)
        {
            m_action = action;
            LeftClicked += m_action;

            OnGetDrawCommands = GetDrawCommands;
        }

        public override Color4? GetFillColor(UIContext uiContext)
        {
            if (uiContext.Active == this)
            {
                return Style.GetColor(StyleColorKey.ButtonFillActive);
            }
            else if (uiContext.Hover == this)
            {
                return Style.GetColor(StyleColorKey.ButtonFillHover);
            }
            else
            {
                return Style.GetColor(StyleColorKey.ButtonFill);
            }
        }

        public override Color4? GetBorderColor(UIContext uiContext)
        {
            if (uiContext.Active == this)
            {
                return this.Style.GetColor(StyleColorKey.ButtonBorderActive);
            }
            else if (uiContext.Hover == this)
            {
                return this.Style.GetColor(StyleColorKey.ButtonBorderHover);
            }
            else
            {
                return this.Style.GetColor(StyleColorKey.ButtonBorder);
            }
        }

        IEnumerable<DrawCommand> GetDrawCommands(UIContext uiContext, RectRegion _)
        {
            return DrawCommandFactory.DrawRectCommands(Rect.ToSharpDX(),
                GetFillColor(uiContext),
                GetBorderColor(uiContext)
                );
        }

        public override void Dispose()
        {
            LeftClicked -= m_action;
            base.Dispose();
        }
    }
}
