using SharpDX;
using System.Collections.Generic;

namespace RectUI
{
    public enum StyleColorKey
    {
        Fill,
        FillHover,
        FillActive,
        Border,
        BorderHover,
        BorderActive,
        Text,
        TextHover,
        TextActive,
    }

    public class Style
    {
        public Dictionary<StyleColorKey, Color4> m_colorMap = new Dictionary<StyleColorKey, Color4>();

        public Style Fallback;
        public Style(Style fallback = null)
        {
            Fallback = fallback ?? Default;
        }

        public Color4 GetColor(StyleColorKey key)
        {
            Color4 color;
            if (m_colorMap.TryGetValue(key, out color))
            {
                return color;
            }
            return Fallback.GetColor(key);
        }

        static Style s_default = new Style(null)
        {
            m_colorMap = new Dictionary<StyleColorKey, Color4>
            {
                {StyleColorKey.Border, new Color4(0.5f, 0.5f, 0.5f, 1) },
                {StyleColorKey.BorderHover, new Color4(1, 0, 0, 1) },
                {StyleColorKey.BorderActive, new Color4(1, 0, 0, 1) },
                {StyleColorKey.Fill, new Color4(0.8f, 0.8f, 0.8f, 1) },
                {StyleColorKey.FillHover, new Color4(1, 1, 1, 1) },
                {StyleColorKey.FillActive, new Color4(1, 1, 0, 1) },
                {StyleColorKey.Text, new Color4(0, 0, 0, 1) },
            }
        };
        public static Style Default
        {
            get
            {
                return s_default;
            }
        }
    }
}
