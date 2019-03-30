using DesktopDll;
using RectUI.Assets;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RectUI.Graphics
{
    class MemoryBitmap : IDisposable
    {
        HDC m_hDC;
        HBITMAP m_bmp;
        public HDC DC
        {
            get;
            private set;
        }
        IntPtr m_hOrgBMP;

        public void Dispose()
        {
            Gdi32.SelectObject(DC, m_hOrgBMP);
            Gdi32.DeleteDC(DC);
            Gdi32.DeleteObject(m_bmp.Value);
            User32.ReleaseDC(default(HWND), m_hDC);
        }

        public MemoryBitmap(int w, int h)
        {
            m_hDC = User32.GetDC(default(HWND));
            m_bmp = Gdi32.CreateCompatibleBitmap(m_hDC, w, h);
            DC = Gdi32.CreateCompatibleDC(m_hDC);
            m_hOrgBMP = Gdi32.SelectObject(DC, m_bmp.Value);
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/gdi/capturing-an-image
        /// </summary>
        public byte[] GetBitmap()
        {
            var bmpScreen = default(BITMAP);
            if(Gdi32.GetObject(m_bmp.Value, Marshal.SizeOf<BITMAP>(), ref bmpScreen) == 0)
            {
                return null;
            }

            //var bmfHeader = default(BITMAPFILEHEADER);

            var bi = default(BITMAPINFOHEADER);
            bi.biSize = Marshal.SizeOf<BITMAPINFOHEADER>();
            bi.biWidth = bmpScreen.bmWidth;
            bi.biHeight = -bmpScreen.bmHeight.Value;
            bi.biPlanes = 1;
            bi.biBitCount = 32;
            bi.biCompression = BI.RGB;
            bi.biSizeImage = 0;
            bi.biXPelsPerMeter = 0;
            bi.biYPelsPerMeter = 0;
            bi.biClrUsed = 0;
            bi.biClrImportant = 0;

            DWORD dwBmpSize = ((bmpScreen.bmWidth.Value * bi.biBitCount.Value + 31) / 32) * 4 * bmpScreen.bmHeight.Value;

            // Starting with 32-bit Windows, GlobalAlloc and LocalAlloc are implemented as wrapper functions that 
            // call HeapAlloc using a handle to the process's default heap. Therefore, GlobalAlloc and LocalAlloc 
            // have greater overhead than HeapAlloc.
            //HANDLE hDIB = GlobalAlloc(GHND, dwBmpSize);
            var lpbitmap = new byte[dwBmpSize.Value];

            // Gets the "bits" from the bitmap and copies them into a buffer 
            // which is pointed to by lpbitmap.
            if(Gdi32.GetDIBits(DC, m_bmp, 
                0, bmpScreen.bmHeight.Value,
                lpbitmap,
                ref bi, DIB.RGB_COLORS) == 0)
            {
                return null;
            }

            return lpbitmap;
        }
    }

    public class D2D1Bitmap : IDisposable
    {
        Bitmap1 _bitmap;
        Dictionary<Color4, SolidColorBrush> _brushMap = new Dictionary<Color4, SolidColorBrush>();
        TextFormat _textFormat;
        Dictionary<IntPtr, Bitmap> _bitmapMap = new Dictionary<IntPtr, Bitmap>();
        Dictionary<int, Bitmap> _imageListMap = new Dictionary<int, Bitmap>();

        D3D11RenderTarget m_renderTarget;
        Dictionary<D3D11RenderTarget, Bitmap> m_d3dbitmapMap = new Dictionary<D3D11RenderTarget, Bitmap>();

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

        public void Draw(D3D11Device device, uint regionID, D2DDrawCommand command)
        {
            switch(command.DrawType)
            {
                case DrawType.Rectangle:
                    DrawRect(device, command.Rectangle, 
                        command.FillColor, command.BorderColor);
                    break;

                case DrawType.Text:
                    DrawText(device, command.Rectangle,
                        command.Font,
                        command.TextColor, command.Text);
                    break;

                case DrawType.Icon:
                    DrawIcon(device, command.Rectangle,
                        command.Icon);
                    break;

                case DrawType.ImageList:
                    DrawImageList(device, command.Rectangle,
                        command.Icon, command.ImageListIndex);
                    break;

                case DrawType.Scene:
                    DrawScene(device, command.Rectangle, command.Camera, command.Scene);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        void DrawRect(D3D11Device device,
            RectangleF rect,
            Color4? fill,
            Color4? border)
        {
            SolidColorBrush fillBrush = null;
            if (fill.HasValue)
            {
                if (!_brushMap.TryGetValue(fill.Value, out fillBrush))
                {
                    fillBrush = new SolidColorBrush(device.D2DDeviceContext, fill.Value);
                    _brushMap.Add(fill.Value, fillBrush);
                }
                device.D2DDeviceContext.FillRectangle(rect, fillBrush);
            }

            SolidColorBrush borderBrush = null;
            if (border.HasValue)
            {
                if (!_brushMap.TryGetValue(border.Value, out borderBrush))
                {
                    borderBrush = new SolidColorBrush(device.D2DDeviceContext, border.Value);
                    _brushMap.Add(border.Value, borderBrush);
                }
                device.D2DDeviceContext.DrawRectangle(rect, borderBrush, 2.0f);
            }
        }

        void DrawIcon(D3D11Device device, 
            RectangleF rect,
            IntPtr icon)
        {
            if (icon == IntPtr.Zero)
            {
                return;
            }
            Bitmap bitmap;
            if(!_bitmapMap.TryGetValue(icon, out bitmap))
            {
                // todo
            }
            // todo
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

        void DrawImageList(D3D11Device device,
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
                        using(var s = new DataStream(w * h * 4, true, true))
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

        void DrawText(D3D11Device device,
            RectangleF rect,
            FontInfo font,
            Color4? textColor,
            TextInfo text)
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
                brush = new SolidColorBrush(device.D2DDeviceContext, textColor.Value);
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

            device.D2DDeviceContext.DrawText(text.Text, _textFormat, rect, brush);
        }       

        RectangleF m_rect;
        void DrawScene(D3D11Device device, RectangleF rect, Camera camera, Scene scene)
        {
            //
            // render 3D scene
            //
            if (rect != m_rect)
            {
                m_rect = rect;
                if (m_renderTarget != null)
                {
                    m_d3dbitmapMap[m_renderTarget].Dispose();
                    m_d3dbitmapMap.Remove(m_renderTarget);

                    m_renderTarget.Dispose();
                    m_renderTarget = null;
                }
            }
            if (m_renderTarget == null)
            {
                m_renderTarget = D3D11RenderTarget.Create(device, (int)rect.Width, (int)rect.Height);
            }

            m_renderTarget.Setup(device, new Color4(0.2f, 0, 0, 1));
            device.SetViewport(new Viewport(0, 0, (int)rect.Width, (int)rect.Height));
            scene.Draw(device, camera.ViewProjection);

            //
            // draw render target
            //
            Bitmap bitmap;
            if (!m_d3dbitmapMap.TryGetValue(m_renderTarget, out bitmap))
            {
                using (var surface = m_renderTarget.Texture.QueryInterface<SharpDX.DXGI.Surface>())
                {
                    bitmap = new Bitmap(device.D2DDeviceContext, surface, GetBP);
                }
                m_d3dbitmapMap.Add(m_renderTarget, bitmap);
            }

            device.D2DDeviceContext.DrawBitmap(bitmap, rect, 1.0f, BitmapInterpolationMode.Linear);
        }
    }
}
