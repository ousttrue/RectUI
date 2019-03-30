using RectUI.Assets;
using SharpDX;
using System;


namespace RectUI.Graphics
{
    public enum DrawType
    {
        None,
        Rectangle,
        Text,
        Icon,
        ImageList,
        Scene,
    }

    public struct FontInfo
    {
        public string Font;
        public float Size;
    }

    public enum TextHorizontalAlignment
    {
        Left,
        Center,
        Right,
    }

    public enum TextVerticalAlignment
    {
        Top,
        Center,
        Bottom,
    }

    public struct TextInfo
    {
        public string Text;
        public TextHorizontalAlignment HorizontalAlignment;
        public TextVerticalAlignment VerticalAlignment;
    }

    public struct Padding
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public float Horizontal => Left + Right;
        public float Vertical => Top + Bottom;
    }

    public struct D2DDrawCommand
    {
        public uint RegionID;
        public RectangleF Rectangle;
        public DrawType DrawType;
        public Color4? FillColor;
        public Color4? BorderColor;
        public Color4? TextColor;
        public TextInfo Text;
        public FontInfo Font;
        public IntPtr Icon;
        public int ImageListIndex;
        public Camera Camera;
        public Scene Scene;
    }
}
