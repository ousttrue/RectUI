using RectUI.Graphics;
using System.Collections.Generic;


namespace RectUI
{
    /// <summary>
    /// RectRegionの内部を描画する
    /// </summary>
    public static class DrawCommandFactory
    {
        public static IEnumerable<DrawCommand> DrawRectCommands(UIContext uiContext, RectRegion r, Style style=null)
        {
            if (style == null)
            {
                style = Style.Default;
            }
            yield return new DrawCommand
            {
                DrawType = DrawType.Rectangle,
                Rectangle = new SharpDX.RectangleF(r.Rect.X, r.Rect.Y, r.Rect.Width, r.Rect.Height),
                FillColor = style.GetFillColor(uiContext, r),
                BorderColor = style.GetBorderColor(uiContext, r),
            };
        }

        public static IEnumerable<DrawCommand> DrawTextCommands(UIContext uiContext, RectRegion r, 
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
                TextColor = style.GetBorderColor(uiContext, r),
                Font = font,
                FontSize = fontSize - topPadding - bottomPadding,
            };
        }
    }
}
