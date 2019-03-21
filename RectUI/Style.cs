using SharpDX;

namespace RectUI
{
    public class Style
    {
        public Color4 FillColor;
        public Color4? FillColorFocus;
        public Color4? FillColorHover;
        public Color4? FillColorActive;
        public Color4 BorderColor;
        public Color4? BorderColorFocus;
        public Color4? BorderColorHover;
        public Color4? BorderColorActive;

        static Style s_thema;
        public static Style Default
        {
            get
            {
                if (s_thema == null)
                {
                    s_thema = new Style
                    {
                        BorderColor = new Color4(0.5f, 0.5f, 0.5f, 1),
                        BorderColorHover = new Color4(1, 0, 0, 1),
                        FillColor = new Color4(0.8f, 0.8f, 0.8f, 1),
                        FillColorHover = new Color4(1, 1, 1, 1),
                        FillColorActive = new Color4(1, 1, 0, 1),
                    };
                }
                return s_thema;
            }
        }

        public Color4 GetBorderColor(UIContext context, RectRegion d)
        {
            if (d == context.Active)
            {
                return BorderColorActive.HasValue ? BorderColorActive.Value : BorderColor;
            }
            else if (context.Active == null && d == context.Hover)
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
