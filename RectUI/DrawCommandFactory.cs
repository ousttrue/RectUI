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
            IntPtr icon)
        {
            yield return new DrawCommand
            {
                DrawType = DrawType.Icon,
                Rectangle = new SharpDX.RectangleF(r.Rect.X, r.Rect.Y, r.Rect.Width, r.Rect.Height),
                Icon = icon
            };
        }

        public static IEnumerable<DrawCommand> DrawImageListCommands(RectRegion r, 
            IntPtr imageList, int imageListIndex)
        {
            yield return new DrawCommand
            {
                DrawType = DrawType.ImageList,
                Rectangle = new SharpDX.RectangleF(r.Rect.X, r.Rect.Y, r.Rect.Width, r.Rect.Height),
                Icon = imageList,
                ImageListIndex = imageListIndex
            };
        }

        public static IEnumerable<DrawCommand> DrawTextCommands(RectangleF rect,
            Padding padding,
            Color4? textColor,
            FontInfo font,
            TextInfo text)
        {
            rect = new RectangleF(
                padding.Left + rect.X,
                padding.Top + rect.Y,
                rect.Width - padding.Left - padding.Right,
                rect.Height -padding.Top - padding.Bottom
            );
            yield return new DrawCommand
            {
                DrawType = DrawType.Text,
                Rectangle = rect,
                Text = text,
                TextColor = textColor,
                Font = font,
            };
        }

        public static IEnumerable<DrawCommand> DrawSceneCommands(RectangleF rect, Camera camera)
        {
            yield return new DrawCommand
            {
                DrawType = DrawType.Scene,
                Rectangle = rect,
                Camera = camera,
            };
        }
    }
}
