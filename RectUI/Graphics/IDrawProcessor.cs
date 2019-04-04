using SharpDX;


namespace RectUI.Graphics
{
    public struct FontInfo
    {
        public readonly string FamilylName;
        public readonly string FaceName;
        public readonly float Size;

        public FontInfo(string family, string face):this(family, face, float.NaN)
        {
        }

        public FontInfo(string family, string face, float size)
        {
            FamilylName = family;
            FaceName = face;
            Size = size;
        }

        public FontInfo Sized(float size)
        {
            return new FontInfo(FamilylName, FaceName, size);
        }

        public static FontInfo MSGothic => new FontInfo("MS Gothic", "Regular");

        public override string ToString()
        {
            return $"{FamilylName}: {FaceName}";
        }
    }

    public struct GridInfo
    {
        public float CellSize;
        public float LineWidth;
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

    public struct TextInfo
    {
        public FontInfo Font;
        public GridInfo? Grid;
        public TextAlignment Alignment;
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
        void FileIcon(uint id, RectangleF rect, string path);
        void Text(uint id, RectangleF rect, string text, Color4? color, TextInfo font);

        void CameraMatrix(uint id, RectangleF rect, Matrix m);
    }
}
