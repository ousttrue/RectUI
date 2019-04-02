using SharpDX;


namespace RectUI.Graphics
{
    public struct FontFaceName
    {
        public string FamilylName;
        public string FaceName;

        public FontFaceName(string family, string face)
        {
            FamilylName = family;
            FaceName = face;
        }

        public static FontFaceName MSGothic => new FontFaceName("MS Gothic", "Regular");

        public override string ToString()
        {
            return $"{FamilylName}: {FaceName}";
        }
    }

    public struct FontInfo
    {
        public FontFaceName Font;
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

    public struct TextAlignment
    {
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
        void Text(uint id, RectangleF rect, Color4? color, FontInfo font, string text, TextAlignment alignment);
        void FileIcon(uint id, RectangleF rect, string path);

        void CameraMatrix(uint id, RectangleF rect, Matrix m);
    }
}
