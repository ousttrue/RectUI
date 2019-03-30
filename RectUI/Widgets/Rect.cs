namespace RectUI.Widgets
{
    public struct Rect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public bool Contains(int x, int y)
        {
            if (x < X) return false;
            if (x > X + Width) return false;
            if (y < Y) return false;
            if (y > Y + Height) return false;
            return true;
        }

        public Rect(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public Rect(int w, int h) : this(0, 0, w, h)
        { }

        public SharpDX.RectangleF ToSharpDX()
        {
            return new SharpDX.RectangleF(X, Y, Width, Height);
        }
    }
}
