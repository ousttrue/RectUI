using SharpDX;
using System;
using System.Collections.Generic;


namespace RectUI.Graphics
{
    /// <summary>
    /// RectRegionの内部を描画する
    /// </summary>
    public static class D2DDrawCommandFactory
    {
        public static IEnumerable<D2DDrawCommand> DrawRectCommands(RectangleF rect, Color4? fill, Color4? border)
        {
            yield return new D2DDrawCommand
            {
                DrawType = DrawType.Rectangle,
                Rectangle = rect,
                FillColor = fill,
                BorderColor = border,
            };
        }

        public static IEnumerable<D2DDrawCommand> DrawIconCommands(RectangleF rect,
            IntPtr icon)
        {
            yield return new D2DDrawCommand
            {
                DrawType = DrawType.Icon,
                Rectangle = rect,
                Icon = icon
            };
        }

        public static IEnumerable<D2DDrawCommand> DrawImageListCommands(RectangleF rect,
            IntPtr imageList, int imageListIndex)
        {
            yield return new D2DDrawCommand
            {
                DrawType = DrawType.ImageList,
                Rectangle = rect,
                Icon = imageList,
                ImageListIndex = imageListIndex
            };
        }

        public static IEnumerable<D2DDrawCommand> DrawTextCommands(RectangleF rect,
            Padding padding,
            Color4? textColor,
            FontInfo font,
            TextInfo text)
        {
            rect = new RectangleF(
                padding.Left + rect.X,
                padding.Top + rect.Y,
                rect.Width - padding.Left - padding.Right,
                rect.Height - padding.Top - padding.Bottom
            );
            yield return new D2DDrawCommand
            {
                DrawType = DrawType.Text,
                Rectangle = rect,
                Text = text,
                TextColor = textColor,
                Font = font,
            };
        }

        public static IEnumerable<D2DDrawCommand> DrawSceneCommands(RectangleF rect, 
            Camera camera,
            Assets.Scene scene)
        {
            yield return new D2DDrawCommand
            {
                DrawType = DrawType.Scene,
                Rectangle = rect,
                Camera = camera,
                Scene = scene,
            };
        }
    }
}
