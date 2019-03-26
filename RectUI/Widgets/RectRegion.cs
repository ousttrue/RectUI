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
            if (m_children != null)
            {
                foreach (var r in m_children)
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

        void Layout()
        {
            foreach(var child in Children)
            {
                var x = child.Rect.X;
                if (child.Anchor.Left.HasValue)
                {
                    x = child.Anchor.Left.Value;
                }

                var y = child.Rect.Y;
                if(child.Anchor.Top.HasValue)
                {
                    y = child.Anchor.Top.Value;
                }

                var w = child.Rect.Width;
                if(child.Anchor.Right.HasValue)
                {
                    w = Rect.Width - child.Anchor.Right.Value - x;
                }

                var h = child.Rect.Height;
                if(child.Anchor.Bottom.HasValue)
                {
                    h = Rect.Height - child.Anchor.Bottom.Value - y;
                }

                child.Rect = new Rect(x, y, w, h);
            }
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
