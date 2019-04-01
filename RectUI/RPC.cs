using RectUI.Graphics;
using RectUI.JSON;
using SharpDX;
using System;


namespace RectUI
{
    public interface IDrawRPC
    {
        void Rectangle(uint id, RectangleF rect, Color4? fill, Color4? border);
        void Text(uint id, RectangleF rect, Color4? color, FontInfo font, TextInfo text);
        void FileIcon(uint id, RectangleF rect, string path);

        void CameraMatrix(uint id, RectangleF rect, Matrix m);
    }

    public class DrawRPCBuffer : IDrawRPC
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

        public void Rectangle(uint id, RectangleF rect, Color4? fill, Color4? border)
        {
            Call(nameof(Rectangle), id, rect, fill, border);
        }

        public void Text(uint id, RectangleF rect, Color4? color, FontInfo font, TextInfo text)
        {
            Call(nameof(Text), id, rect, color, font, text);
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
