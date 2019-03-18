namespace RectUI
{
    public struct Rect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public bool Include(int x, int y)
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
    }
}
