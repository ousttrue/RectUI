using SharpDX;

namespace RectUI
{
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

        public Color4 GetBorderColor(UIContext context, RectRegion d)
        {
            if (d == context.Active)
            {
                return BorderColorActive.HasValue ? BorderColorActive.Value : BorderColor;
            }
            else if (context.Active==null && d == context.Hover)
            {
                return BorderColorHover.HasValue ? BorderColorHover.Value : BorderColor;
            }
            return BorderColor;
        }

        public Color4 GetFillColor(UIContext context, RectRegion d)
        {
            if (d == context.Active)
            {
                return FillColorActive.HasValue ? FillColorActive.Value : FillColor;
            }
            else if (context.Active == null && d == context.Hover)
            {
                return FillColorHover.HasValue ? FillColorHover.Value : FillColor;
            }
            return FillColor;
        }
    }
}
