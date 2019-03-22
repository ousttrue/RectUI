using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;


namespace RectUI.Graphics
{
    public class D2D1Bitmap : IDisposable
    {
        Bitmap1 _bitmap;
        Dictionary<Color4, SolidColorBrush> _brushMap = new Dictionary<Color4, SolidColorBrush>();
        TextFormat _textFormat;

        public void Dispose()
        {
            if (_textFormat != null)
            {
                _textFormat.Dispose();
                _textFormat = null;
            }

            foreach (var kv in _brushMap)
            {
                kv.Value.Dispose();
            }
            _brushMap.Clear();

            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
        }

        Func<SharpDX.DXGI.Surface> _getSurface;
        public D2D1Bitmap(Func<SharpDX.DXGI.Surface> getSurface)
        {
            _getSurface = getSurface;
        }

        void CreateBitmap(D3D11Device device)
        {
            Dispose();

            var pf = new PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Ignore);
            var bp = new BitmapProperties1(pf, device.Dpi.Height, device.Dpi.Width,
                BitmapOptions.CannotDraw | BitmapOptions.Target)
                ;

            using (var surface = _getSurface())
            {
                _bitmap = new Bitmap1(device.D2DDeviceContext, surface);
            }
        }

        public void Begin(D3D11Device device, Color4 clear)
        {
            if (_bitmap == null)
            {
                CreateBitmap(device);
            }

            device.D2DDeviceContext.Target = _bitmap;
            device.D2DDeviceContext.BeginDraw();
            device.D2DDeviceContext.Clear(clear);
            device.D2DDeviceContext.Transform = Matrix3x2.Identity;
        }

        public void End(D3D11Device device)
        {
            device.D2DDeviceContext.Target = null;
            device.D2DDeviceContext.EndDraw();
        }

        public void Draw(D3D11Device device, DrawCommand command)
        {
            switch(command.DrawType)
            {
                case DrawType.Rectangle:
                    DrawRect(device, command.Rectangle, 
                        command.FillColor, command.BorderColor);
                    break;

                case DrawType.Text:
                    DrawText(device, command.Rectangle, 
                        command.Font, command.FontSize,
                        command.TextColor, command.Text);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        void DrawRect(D3D11Device device,
            RectangleF rect,
            Color4 fill,
            Color4 border)
        {
            SolidColorBrush fillBrush;
            if (!_brushMap.TryGetValue(fill, out fillBrush))
            {
                fillBrush = new SolidColorBrush(device.D2DDeviceContext, fill);
                _brushMap.Add(fill, fillBrush);
            }

            SolidColorBrush borderBrush;
            if (!_brushMap.TryGetValue(border, out borderBrush))
            {
                borderBrush = new SolidColorBrush(device.D2DDeviceContext, border);
                _brushMap.Add(border, borderBrush);
            }

            device.D2DDeviceContext.FillRectangle(rect, fillBrush);
            device.D2DDeviceContext.DrawRectangle(rect, borderBrush, 2.0f);
        }

        void DrawText(D3D11Device device,
            RectangleF rect,
            string font,
            float fontSize,
            Color4 textColor,
            string text)
        {
            SolidColorBrush brush;
            if (!_brushMap.TryGetValue(textColor, out brush))
            {
                brush = new SolidColorBrush(device.D2DDeviceContext, textColor);
                _brushMap.Add(textColor, brush);
            }

            if (_textFormat == null)
            {
                using (var f = new SharpDX.DirectWrite.Factory())
                {
                    _textFormat = new TextFormat(f, font, fontSize);
                }
            }

            device.D2DDeviceContext.DrawText(text, _textFormat, rect, brush);
        }
    }
}
