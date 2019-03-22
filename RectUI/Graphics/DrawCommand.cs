using SharpDX;

namespace RectUI.Graphics
{
    public enum DrawType
    {
        Rectangle,
        Text,
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
    }
}
