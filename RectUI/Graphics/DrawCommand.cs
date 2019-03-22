using SharpDX;
using System;

namespace RectUI.Graphics
{
    public enum DrawType
    {
        Rectangle,
        Text,
        Icon,
        ImageList,
    }

    public struct DrawCommand
    {
        public DrawType DrawType;
        public RectangleF Rectangle;
        public Color4 FillColor;
        public Color4 BorderColor;
        public Color4 TextColor;
        public string Text;
        public string Font;
        public float FontSize;
        public IntPtr Icon;
        public int ImageListIndex;
    }
}
