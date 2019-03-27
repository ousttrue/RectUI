using RectUI.Graphics;
using RectUI.Widgets;
using SharpDX;
using System;
using System.Collections.Generic;


namespace RectUI
{
    /// <summary>
    /// RectRegionの内部を描画する
    /// </summary>
    public static class DrawCommandFactory
    {
        public static IEnumerable<DrawCommand> DrawRectCommands(RectangleF rect, Color4? fill, Color4? border)
        {
            yield return new DrawCommand
            {
                DrawType = DrawType.Rectangle,
                Rectangle = rect,
                FillColor = fill,
                BorderColor = border,
            };
        }

        public static IEnumerable<DrawCommand> DrawIconCommands(UIContext uiContext, RectRegion r, 
            IntPtr icon, Style style = null)
        {
            if (style == null)
            {
                style = Style.Default;
            }
            yield return new DrawCommand
            {
                DrawType = DrawType.Icon,
                Rectangle = new SharpDX.RectangleF(r.Rect.X, r.Rect.Y, r.Rect.Width, r.Rect.Height),
                Icon = icon
            };
        }

        public static IEnumerable<DrawCommand> DrawImageListCommands(RectRegion r, 
            IntPtr imageList, int imageListIndex, Style style = null)
        {
            if (style == null)
            {
                style = Style.Default;
            }
            yield return new DrawCommand
            {
                DrawType = DrawType.ImageList,
                Rectangle = new SharpDX.RectangleF(r.Rect.X, r.Rect.Y, r.Rect.Width, r.Rect.Height),
                Icon = imageList,
                ImageListIndex = imageListIndex
            };
        }

        public static IEnumerable<DrawCommand> DrawTextCommands(RectRegion r, 
            Color4? textColor,
            string font, float fontSize, 
            float leftPadding, float topPadding, float rightPadding, float bottomPadding,
            string text, Style style=null)
        {
            if (style == null)
            {
                style = Style.Default;
            }
            yield return new DrawCommand
            {
                DrawType = DrawType.Text,
                Rectangle = new SharpDX.RectangleF(
                    r.Rect.X + leftPadding, 
                    r.Rect.Y + topPadding, 
                    r.Rect.Width - leftPadding - rightPadding, 
                    r.Rect.Height),
                Text = text,
                TextColor = textColor,
                Font = font,
                FontSize = fontSize - topPadding - bottomPadding,
            };
        }
    }
}
