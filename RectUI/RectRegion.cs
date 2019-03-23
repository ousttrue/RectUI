using System;
using System.Collections.Generic;
using RectUI.Graphics;


namespace RectUI
{
    public delegate IEnumerable<DrawCommand> GetDrawCommandsFunc(UIContext uiContext, RectRegion r);

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

        public GetDrawCommandsFunc OnGetDrawCommands = (uiContext, r) =>
        {
            return DrawCommandFactory.DrawRectCommands(r.Rect.ToSharpDX(),
                Style.Default.GetFillColor(uiContext, r),
                Style.Default.GetBorderColor(uiContext, r));
        };

        public RectRegion Parent
        {
            get;
            set;
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
            if (Rect.Include(x, y))
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

        public event Action<RectRegion, int> OnWheel;
        public bool Wheel(RectRegion sender, int delta)
        {
            if (OnWheel == null) return false;
            OnWheel(sender, delta);
            return true;
        }
    }

    public class ContentRegion<T>: RectRegion
    {
        public T Content
        {
            get;
            set;
        }
    }
}
