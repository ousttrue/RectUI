using SharpDX;
using System.Collections.Generic;


namespace RectUI
{
    public struct Rect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public bool Include(int x, int y)
        {
            if (x < X) return false;
            if (x > X + Width) return false;
            if (y < Y) return false;
            if (y > Y + Height) return false;
            return true;
        }

        public Rect(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
    }

    public class Thema
    {
        public Color4 FillColor;
        public Color4? FillColorFocus;
        public Color4? FillColorHover;
        public Color4? FillColorActive;
        public Color4 BorderColor;
        public Color4? BorderColorFocus;
        public Color4? BorderColorHover;
        public Color4? BorderColorActive;

        public Color4 GetBorderColor(DrawInfo d)
        {
            if (d.IsActive)
            {
                return BorderColorActive.HasValue ? BorderColorActive.Value : BorderColor;
            }
            else if (d.IsHover)
            {
                return BorderColorHover.HasValue ? BorderColorHover.Value : BorderColor;
            }
            return BorderColor;
        }

        public Color4 GetFillColor(DrawInfo d)
        {
            if (d.IsActive)
            {
                return FillColorActive.HasValue ? FillColorActive.Value : FillColor;
            }
            else if (d.IsHover)
            {
                return FillColorHover.HasValue ? FillColorHover.Value : FillColor;
            }
            return FillColor;
        }
    }

    public class DrawInfo
    {
        public Rect Rect;
        public bool HasFocus;
        public bool IsHover;
        public bool MouseLeftDown;
        public bool MouseRightDown;
        public bool MouseMiddleDown;
        public bool IsActive
        {
            get
            {
                return MouseLeftDown || MouseRightDown || MouseMiddleDown;
            }
        }
        public int MouseX;
        public int MouseY;
        public bool IsMouseInclude
        {
            get
            {
                return Rect.Include(MouseX, MouseY);
            }
        }

        public void MouseMove(int x, int y)
        {
            IsHover = Rect.Include(x, y);
            MouseX = x;
            MouseY = y;
        }
    }
}
