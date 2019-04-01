using DesktopDll;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;

namespace RectUI.Graphics
{
    public class D2D1Bitmap : IDisposable, IDrawProcessor
    {
        public D3D11Device Device
        {
            get;
            set;
        }

        Bitmap1 _bitmap;
        Dictionary<Color4, SolidColorBrush> _brushMap = new Dictionary<Color4, SolidColorBrush>();
        TextFormat _textFormat;
        Dictionary<IntPtr, Bitmap> _bitmapMap = new Dictionary<IntPtr, Bitmap>();
        Dictionary<int, Bitmap> _imageListMap = new Dictionary<int, Bitmap>();
        Dictionary<D3D11RenderTarget, Bitmap> m_rtBitmapMap = new Dictionary<D3D11RenderTarget, Bitmap>();

        public void Dispose()
        {
            foreach(var kv in m_rtBitmapMap)
            {
                kv.Value.Dispose();
            }
            m_rtBitmapMap.Clear();

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

        public void Begin(Color4 clear)
        {
            if (_bitmap == null)
            {
                CreateBitmap(Device);
            }

            Device.D2DDeviceContext.Target = _bitmap;
            Device.D2DDeviceContext.BeginDraw();
            Device.D2DDeviceContext.Clear(clear);
            Device.D2DDeviceContext.Transform = Matrix3x2.Identity;
        }

        public void End()
        {
            Device.D2DDeviceContext.Target = null;
            Device.D2DDeviceContext.EndDraw();
        }

        BitmapProperties GetBP
        {
            get
            {
                var bp = new BitmapProperties
                {
                    PixelFormat = new PixelFormat
                    {
                        Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                        AlphaMode = AlphaMode.Premultiplied
                    }
                };
                return bp;
            }
        }

        public void DrawImageList(D3D11Device device,
            RectangleF rect,
            IntPtr imageList, int imageListIndex)
        {
            if (imageList == IntPtr.Zero)
            {
                return;
            }

            int w = 0;
            int h = 0;
            if (!Comctl32.ImageList_GetIconSize(imageList, ref w, ref h))
            {
                return;
            }

            Bitmap bitmap;
            if (!_imageListMap.TryGetValue(imageListIndex, out bitmap))
            {
                using (var memoryBitmap = new MemoryBitmap(w, h))
                {
                    Comctl32.ImageList_Draw(imageList, imageListIndex, memoryBitmap.DC, 0, 0, ILD.NORMAL);

                    var bytes = memoryBitmap.GetBitmap();
                    if (bytes != null)
                    {
                        using (var s = new DataStream(w * h * 4, true, true))
                        {
                            s.Write(bytes, 0, bytes.Length);
                            s.Position = 0;
                            bitmap = new Bitmap(device.D2DDeviceContext, new Size2(w, h), s, w * 4, GetBP);
                            _imageListMap.Add(imageListIndex, bitmap);
                        }
                    }
                }
            }

            device.D2DDeviceContext.DrawBitmap(bitmap, new RectangleF(rect.X, rect.Y, w, h), 1.0f, BitmapInterpolationMode.Linear);
        }

        Bitmap GetOrCreateBitmap(D3D11Device device, D3D11RenderTarget renderTarget)
        {
            Bitmap bitmap;
            if (!m_rtBitmapMap.TryGetValue(renderTarget, out bitmap))
            {
                using (var surface = renderTarget.Texture.QueryInterface<SharpDX.DXGI.Surface>())
                {
                    bitmap = new Bitmap(device.D2DDeviceContext, surface, GetBP);
                }
            }
            return bitmap;
        }

        public void DrawRenderTarget(D3D11Device device, RectangleF rect, 
            D3D11RenderTarget renderTarget)
        {
            var bitmap = GetOrCreateBitmap(device, renderTarget);
            device.D2DDeviceContext.DrawBitmap(bitmap, rect, 1.0f, BitmapInterpolationMode.Linear);
        }

        public void Rectangle(uint id, RectangleF rect, Color4? fill, Color4? border)
        {
            SolidColorBrush fillBrush = null;
            if (fill.HasValue)
            {
                if (!_brushMap.TryGetValue(fill.Value, out fillBrush))
                {
                    fillBrush = new SolidColorBrush(Device.D2DDeviceContext, fill.Value);
                    _brushMap.Add(fill.Value, fillBrush);
                }
                Device.D2DDeviceContext.FillRectangle(rect, fillBrush);
            }

            SolidColorBrush borderBrush = null;
            if (border.HasValue)
            {
                if (!_brushMap.TryGetValue(border.Value, out borderBrush))
                {
                    borderBrush = new SolidColorBrush(Device.D2DDeviceContext, border.Value);
                    _brushMap.Add(border.Value, borderBrush);
                }
                Device.D2DDeviceContext.DrawRectangle(rect, borderBrush, 2.0f);
            }
        }

        public void Text(uint id, RectangleF rect, Color4? textColor, FontInfo font, TextInfo text)
        {
            if (!textColor.HasValue)
            {
                return;
            }
            if (string.IsNullOrEmpty(text.Text))
            {
                return;
            }

            SolidColorBrush brush;
            if (!_brushMap.TryGetValue(textColor.Value, out brush))
            {
                brush = new SolidColorBrush(Device.D2DDeviceContext, textColor.Value);
                _brushMap.Add(textColor.Value, brush);
            }

            if (_textFormat == null)
            {
                using (var f = new SharpDX.DirectWrite.Factory())
                {
                    _textFormat = new TextFormat(f, font.Font, font.Size);
                }

                switch (text.HorizontalAlignment)
                {
                    case TextHorizontalAlignment.Left:
                        _textFormat.TextAlignment = TextAlignment.Leading;
                        break;

                    case TextHorizontalAlignment.Center:
                        _textFormat.TextAlignment = TextAlignment.Center;
                        break;

                    case TextHorizontalAlignment.Right:
                        _textFormat.TextAlignment = TextAlignment.Trailing;
                        break;
                }

                switch (text.VerticalAlignment)
                {
                    case TextVerticalAlignment.Top:
                        _textFormat.ParagraphAlignment = ParagraphAlignment.Near;
                        break;

                    case TextVerticalAlignment.Center:
                        _textFormat.ParagraphAlignment = ParagraphAlignment.Center;
                        break;

                    case TextVerticalAlignment.Bottom:
                        _textFormat.ParagraphAlignment = ParagraphAlignment.Far;
                        break;
                }
            }

            Device.D2DDeviceContext.DrawText(text.Text, _textFormat, rect, brush);
        }

        public void FileIcon(uint id, RectangleF rect, string path)
        {
            //throw new NotImplementedException();
        }

        public void CameraMatrix(uint id, RectangleF rect, Matrix m)
        {
            //throw new NotImplementedException();
        }
    }
}
