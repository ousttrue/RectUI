using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RectUI.Widgets
{
    public struct ColorKeys
    {
        public StyleColorKey FillColorKey;
        public StyleColorKey BorderColorKey;
        public StyleColorKey TextColorKey;

        public static ColorKeys PanelNormal => new ColorKeys
        {
            FillColorKey = StyleColorKey.PanelFill,
            BorderColorKey = StyleColorKey.PanelBorder,
            TextColorKey = StyleColorKey.Text,
        };

        public static ColorKeys ButtonNormal => new ColorKeys
        {
            FillColorKey = StyleColorKey.ButtonFill,
            BorderColorKey = StyleColorKey.ButtonBorder,
            TextColorKey = StyleColorKey.Text,
        };

        public static ColorKeys ButtonHover => new ColorKeys
        {
            FillColorKey = StyleColorKey.ButtonFillHover,
            BorderColorKey = StyleColorKey.ButtonBorderHover,
            TextColorKey = StyleColorKey.Text,
        };

        public static ColorKeys ButtonActive => new ColorKeys
        {
            FillColorKey = StyleColorKey.ButtonFillActive,
            BorderColorKey = StyleColorKey.ButtonBorderActive,
            TextColorKey = StyleColorKey.Text,
        };

        public static ColorKeys ListItemNormal => new ColorKeys
        {
            FillColorKey = StyleColorKey.ListItemFill,
            BorderColorKey = StyleColorKey.ListItemBorder,
            TextColorKey = StyleColorKey.Text,
        };

        public static ColorKeys ListItemHover => new ColorKeys
        {
            FillColorKey = StyleColorKey.ListItemFillHover,
            BorderColorKey = StyleColorKey.ListItemBorderHover,
            TextColorKey = StyleColorKey.Text,
        };

        public static ColorKeys ListItemActive => new ColorKeys
        {
            FillColorKey = StyleColorKey.ListItemFillActive,
            BorderColorKey = StyleColorKey.ListItemBorderActive,
            TextColorKey = StyleColorKey.Text,
        };
    }
}
