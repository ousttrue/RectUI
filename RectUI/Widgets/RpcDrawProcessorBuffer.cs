using RectUI.Graphics;
using RectUI.JSON;
using SharpDX;
using System;


namespace RectUI.Widgets
{
    public class RpcDrawProcessorBuffer : IDrawProcessor
    {
        MsgPackFormatter m_f = new MsgPackFormatter();
        public void Clear()
        {
            m_f.Clear();
        }
        public ArraySegment<byte> MsgPackBytes
        {
            get { return m_f.GetStoreBytes(); }
        }

        void Call<A0, A1, A2>(string method, A0 a0, A1 a1, A2 a2)
        {
            m_f.Notify(method, a0, a1, a2);
        }
        void Call<A0, A1, A2, A3>(string method, A0 a0, A1 a1, A2 a2, A3 a3)
        {
            m_f.Notify(method, a0, a1, a2, a3);
        }
        void Call<A0, A1, A2, A3, A4>(string method, A0 a0, A1 a1, A2 a2, A3 a3, A4 a4)
        {
            m_f.Notify(method, a0, a1, a2, a3, a4);
        }
        void Call<A0, A1, A2, A3, A4, A5>(string method, A0 a0, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
        {
            m_f.Notify(method, a0, a1, a2, a3, a4, a5);
        }

        public void Rectangle(uint id, RectangleF rect, Color4? fill, Color4? border)
        {
            Call(nameof(Rectangle), id, rect, fill, border);
        }

        public void Grid(uint id, RectangleF rect, Color4? fill, Color4? border, GridInfo grid)
        {
            Call(nameof(Grid), id, rect, fill, border, grid);
        }

        public void Text(uint id, RectangleF rect, Color4? color, FontInfo font, string text, TextAlignment alignment)
        {
            Call(nameof(Text), id, rect, color, font, text, alignment);
        }

        public void CameraMatrix(uint id, RectangleF rect, Matrix m)
        {
            Call(nameof(CameraMatrix), id, rect, m);
        }

        public void FileIcon(uint id, RectangleF rect, string path)
        {
            Call(nameof(FileIcon), id, rect, path);
        }
    }
}
