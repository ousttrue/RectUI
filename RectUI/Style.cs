using SharpDX;
using System.Collections.Generic;

namespace RectUI
{
    public enum StyleColorKey
    {
        PanelFill,
        PanelBorder,
        Text,

        ButtonFill,
        ButtonFillHover,
        ButtonFillActive, // press
        ButtonBorder,
        ButtonBorderHover,
        ButtonBorderActive, // press

        ListItemFill,
        ListItemFillHover,
        ListItemFillActive, // select
        ListItemBorder,
        ListItemBorderHover,
        ListItemBorderActive, // select
    }

    public class Style
    {
        public Dictionary<StyleColorKey, Color4> m_colorMap = new Dictionary<StyleColorKey, Color4>();

        public Style Fallback;
        public Style(Style fallback = null)
        {
            Fallback = fallback ?? Default;
        }

        public Color4? GetColor(StyleColorKey key)
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
                {StyleColorKey.PanelBorder, new Color4(0.5f, 0.5f, 0.5f, 1) },
                {StyleColorKey.PanelFill, new Color4(0.8f, 0.8f, 0.8f, 1) },

                {StyleColorKey.ButtonBorder, new Color4(0.5f, 0.5f, 0.5f, 1) },
                {StyleColorKey.ButtonBorderHover, new Color4(1, 0, 0, 1) },
                {StyleColorKey.ButtonBorderActive, new Color4(1, 0, 0, 1) },
                {StyleColorKey.ButtonFill, new Color4(0.8f, 0.8f, 0.8f, 1) },
                {StyleColorKey.ButtonFillHover, new Color4(1, 1, 1, 1) },
                {StyleColorKey.ButtonFillActive, new Color4(1, 1, 0, 1) },

                {StyleColorKey.ListItemBorder, new Color4(0.5f, 0.5f, 0.5f, 1) },
                {StyleColorKey.ListItemBorderHover, new Color4(1, 0, 0, 1) },
                {StyleColorKey.ListItemBorderActive, new Color4(1, 0, 0, 1) },
                {StyleColorKey.ListItemFill, new Color4(0.8f, 0.8f, 0.8f, 1) },
                {StyleColorKey.ListItemFillHover, new Color4(1, 1, 1, 1) },
                {StyleColorKey.ListItemFillActive, new Color4(1, 1, 0, 1) },

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
