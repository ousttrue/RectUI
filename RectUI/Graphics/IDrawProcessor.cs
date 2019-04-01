using SharpDX;


namespace RectUI.Graphics
{
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

    public interface IDrawProcessor
    {
        void Rectangle(uint id, RectangleF rect, Color4? fill, Color4? border);
        void Text(uint id, RectangleF rect, Color4? color, FontInfo font, TextInfo text);
        void FileIcon(uint id, RectangleF rect, string path);

        void CameraMatrix(uint id, RectangleF rect, Matrix m);
    }
}
