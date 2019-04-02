using DesktopDll;
using RectUI.Application;
using RectUI.Assets;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;

namespace RectUI.Graphics
{
    public class D2D1Bitmap : IDisposable, IDrawProcessor
    {
        D3D11Device m_device;

        Bitmap1 m_bitmap;
        Dictionary<Color4, SolidColorBrush> m_brushMap = new Dictionary<Color4, SolidColorBrush>();
        Dictionary<FontInfo, TextFormat> m_formatMap = new Dictionary<FontInfo, TextFormat>();
        Dictionary<IntPtr, Bitmap> m_bitmapMap = new Dictionary<IntPtr, Bitmap>();
        Dictionary<int, Bitmap> m_imageListMap = new Dictionary<int, Bitmap>();
        Dictionary<D3D11RenderTarget, Bitmap> m_rtBitmapMap = new Dictionary<D3D11RenderTarget, Bitmap>();

        public void Dispose()
        {
            if (m_renderTarget != null)
            {
                m_renderTarget.Dispose();
                m_renderTarget = null;
            }

            foreach (var kv in m_rtBitmapMap)
            {
                kv.Value.Dispose();
            }
            m_rtBitmapMap.Clear();

            foreach(var kv in m_formatMap)
            { 
                kv.Value.Dispose();
            }
            m_formatMap.Clear();

            foreach (var kv in m_brushMap)
            {
                kv.Value.Dispose();
            }
            m_brushMap.Clear();

            if (m_bitmap != null)
            {
                m_bitmap.Dispose();
                m_bitmap = null;
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
                m_bitmap = new Bitmap1(device.D2DDeviceContext, surface);
            }
        }

        public void Begin(D3D11Device device, Scene scene, Color4 clear)
        {
            m_device = device;
            m_scene = scene;

            if (m_bitmap == null)
            {
                CreateBitmap(m_device);
            }

            m_device.D2DDeviceContext.Target = m_bitmap;
            m_device.D2DDeviceContext.BeginDraw();
            m_device.D2DDeviceContext.Clear(clear);
            m_device.D2DDeviceContext.Transform = Matrix3x2.Identity;
        }

        public void End()
        {
            m_device.D2DDeviceContext.Target = null;
            m_device.D2DDeviceContext.EndDraw();
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

        public void Rectangle(uint id, RectangleF rect, Color4? fill, Color4? border)
        {
            SolidColorBrush fillBrush = null;
            if (fill.HasValue)
            {
                if (!m_brushMap.TryGetValue(fill.Value, out fillBrush))
                {
                    fillBrush = new SolidColorBrush(m_device.D2DDeviceContext, fill.Value);
                    m_brushMap.Add(fill.Value, fillBrush);
                }
                m_device.D2DDeviceContext.FillRectangle(rect, fillBrush);
            }

            SolidColorBrush borderBrush = null;
            if (border.HasValue)
            {
                if (!m_brushMap.TryGetValue(border.Value, out borderBrush))
                {
                    borderBrush = new SolidColorBrush(m_device.D2DDeviceContext, border.Value);
                    m_brushMap.Add(border.Value, borderBrush);
                }
                m_device.D2DDeviceContext.DrawRectangle(rect, borderBrush, 2.0f);
            }
        }

        public void Text(uint id, RectangleF rect, Color4? textColor, FontInfo font, string text, TextAlignment alignment)
        {
            if (!textColor.HasValue)
            {
                return;
            }
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            SolidColorBrush brush;
            if (!m_brushMap.TryGetValue(textColor.Value, out brush))
            {
                brush = new SolidColorBrush(m_device.D2DDeviceContext, textColor.Value);
                m_brushMap.Add(textColor.Value, brush);
            }

            TextFormat textFormat;
            if(!m_formatMap.TryGetValue(font, out textFormat)) { 
                using (var f = new SharpDX.DirectWrite.Factory())
                {
                    textFormat = new TextFormat(f, font.Font.FamilylName, font.Size);
                }

                switch (alignment.HorizontalAlignment)
                {
                    case TextHorizontalAlignment.Left:
                        textFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
                        break;

                    case TextHorizontalAlignment.Center:
                        textFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
                        break;

                    case TextHorizontalAlignment.Right:
                        textFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
                        break;
                }

                switch (alignment.VerticalAlignment)
                {
                    case TextVerticalAlignment.Top:
                        textFormat.ParagraphAlignment = ParagraphAlignment.Near;
                        break;

                    case TextVerticalAlignment.Center:
                        textFormat.ParagraphAlignment = ParagraphAlignment.Center;
                        break;

                    case TextVerticalAlignment.Bottom:
                        textFormat.ParagraphAlignment = ParagraphAlignment.Far;
                        break;
                }
            }

            m_device.D2DDeviceContext.DrawText(text, textFormat, rect, brush);
        }

        public void FileIcon(uint id, RectangleF rect, string path)
        {
            var systemIcon = SystemIcon.Get(path, true);

            //throw new NotImplementedException();
            if (systemIcon.ImageList == IntPtr.Zero)
            {
                return;
            }

            int w = 0;
            int h = 0;
            if (!Comctl32.ImageList_GetIconSize(systemIcon.ImageList, ref w, ref h))
            {
                return;
            }

            Bitmap bitmap;
            if (!m_imageListMap.TryGetValue(systemIcon.ImageListIndex, out bitmap))
            {
                using (var memoryBitmap = new MemoryBitmap(w, h))
                {
                    Comctl32.ImageList_Draw(systemIcon.ImageList, systemIcon.ImageListIndex,
                        memoryBitmap.DC, 0, 0, ILD.NORMAL);

                    var bytes = memoryBitmap.GetBitmap();
                    if (bytes != null)
                    {
                        using (var s = new DataStream(w * h * 4, true, true))
                        {
                            s.Write(bytes, 0, bytes.Length);
                            s.Position = 0;
                            bitmap = new Bitmap(m_device.D2DDeviceContext, new Size2(w, h), s, w * 4, GetBP);
                            m_imageListMap.Add(systemIcon.ImageListIndex, bitmap);
                        }
                    }
                }
            }

            m_device.D2DDeviceContext.DrawBitmap(bitmap, new RectangleF(rect.X, rect.Y, w, h), 1.0f, BitmapInterpolationMode.Linear);
        }

        #region Scene
        D3D11RenderTarget m_renderTarget;
        Scene m_scene;
        RectangleF m_rect;
        void GetOrRenderTarget(D3D11Device device, uint id, RectangleF rect)
        {
            if (rect != m_rect)
            {
                m_rect = rect;
                if (m_renderTarget != null)
                {
                    m_renderTarget.Dispose();
                    m_renderTarget = null;
                }
            }
            if (m_renderTarget == null)
            {
                m_renderTarget = D3D11RenderTarget.Create(device, (int)rect.Width, (int)rect.Height);
            }
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

        public void CameraMatrix(uint id, RectangleF rect, Matrix m)
        {
            // render 3D scene
            GetOrRenderTarget(m_device, id, rect);
            m_renderTarget.Setup(m_device, new Color4(0.2f, 0, 0, 1));
            m_device.SetViewport(new Viewport(0, 0,
                (int)rect.Width,
                (int)rect.Height));
            m_scene.Draw(m_device, m);

            var bitmap = GetOrCreateBitmap(m_device, m_renderTarget);
            m_device.D2DDeviceContext.DrawBitmap(bitmap, rect, 1.0f, BitmapInterpolationMode.Linear);
        }
        #endregion
    }
}
