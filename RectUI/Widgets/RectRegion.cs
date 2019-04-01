using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RectUI.Graphics;
using SharpDX;

namespace RectUI.Widgets
{
    public enum DragEvent
    {
        Begin,
        Drag,
        End,
    }

    public class RectRegion : IEnumerable<RectRegion>, IDisposable
    {
        public event Action Invalidated;
        public void Invalidate()
        {
            Root.Invalidated?.Invoke();
        }

        #region ID
        static uint s_id = 1;
        public uint ID
        {
            get;
            private set;
        }
        public RectRegion()
        {
            ID = s_id++;
        }
        #endregion

        #region IEnumerable<RectRegion>
        public virtual void Dispose()
        {
            if (m_children != null)
            {
                foreach (var child in m_children)
                {
                    child.Dispose();
                }
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

        public RectRegion Root
        {
            get
            {
                var root = this;
                while (root.Parent != null)
                {
                    root = root.Parent;
                }
                return root;
            }
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

        #region Layout
        Rect m_rect;
        public Rect Rect
        {
            get { return m_rect; }
            set
            {
                if (m_rect.Equals(value)) return;
                m_rect = value;
                Layout();
            }
        }

        protected virtual void Layout()
        {

        }
        #endregion

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
            if (isActive && ActiveColor.HasValue)
            {
                var color = Style.GetColor(ActiveColor.Value.FillColorKey);
                if (color.HasValue)
                {
                    return color;
                }
            }

            if ((isActive || isHover) && HoverColor.HasValue)
            {
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

        public virtual void GetDrawCommands(IDrawProcessor rpc, bool isActive, bool isHover)
        {
            rpc.Rectangle(ID, Rect.ToSharpDX(), 
                GetFillColor(isActive, isHover), 
                GetBorderColor(isActive, isHover));
        }
        #endregion

        #region MouseEvents
        public virtual RectRegion MouseMove(int x, int y)
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

        public event Action<RectRegion> MouseLeftClicked;
        public void MouseLeftClick(RectRegion sender)
        {
            MouseLeftClicked?.Invoke(sender);
        }

        public event Action<RectRegion> MouseLeftDoubleClicked;
        public bool MouseLeftDoubleClick(RectRegion sender)
        {
            if (MouseLeftDoubleClicked == null) return false;
            MouseLeftDoubleClicked?.Invoke(sender);
            return true;
        }

        public event Action<RectRegion, DragEvent, int, int> MouseLeftDragged;
        public bool MouseLeftDrag(RectRegion sender, DragEvent dragEvent, int x, int y)
        {
            if (MouseLeftDragged == null) return false;
            MouseLeftDragged(sender, dragEvent, x, y);
            return true;
        }

        public event Action<RectRegion, DragEvent, int, int> MouseRightDragged;
        public bool MouseRightDrag(RectRegion sender, DragEvent dragEvent, int x, int y)
        {
            if (MouseRightDragged == null) return false;
            MouseRightDragged(sender, dragEvent, x, y);
            return true;
        }

        public event Action<RectRegion, DragEvent, int, int> MouseMiddleDragged;
        public bool MouseMiddleDrag(RectRegion sender, DragEvent dragEvent, int x, int y)
        {
            if (MouseMiddleDragged == null) return false;
            MouseMiddleDragged(sender, dragEvent, x, y);
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
}
