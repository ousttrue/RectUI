using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;


namespace Graphics
{
    public class D2D1Bitmap : IDisposable
    {
        Bitmap1 _bitmap;
        Dictionary<Color4, SolidColorBrush> _brushMap = new Dictionary<Color4, SolidColorBrush>();

        public void Dispose()
        {
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

        void Create(D3D11Device device)
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
                Create(device);
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

        public void DrawRect(D3D11Device device, int x, int y, int w, int h, 
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
            if(!_brushMap.TryGetValue(border, out borderBrush))
            {
                borderBrush = new SolidColorBrush(device.D2DDeviceContext, border);
                _brushMap.Add(border, borderBrush);
            }

            device.D2DDeviceContext.FillRectangle(new RectangleF(x, y, w, h), fillBrush);
            device.D2DDeviceContext.DrawRectangle(new RectangleF(x, y, w, h), borderBrush, 2.0f);
        }
    }
}
