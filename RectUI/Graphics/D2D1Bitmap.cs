using DesktopDll;
using RectUI.Application;
using RectUI.Assets;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;


namespace RectUI.Graphics
{
    public class D2D1Bitmap : IDisposable, IDrawProcessor
    {
        D3D11Device m_device;

        Bitmap1 m_bitmap;
        Dictionary<Color4, SolidColorBrush> m_brushMap = new Dictionary<Color4, SolidColorBrush>();
        Dictionary<FontInfo, TextFormat> m_formatMap = new Dictionary<FontInfo, TextFormat>();
        Dictionary<FontInfo, FontFace> m_faceMap = new Dictionary<FontInfo, FontFace>();
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

            foreach (var kv in m_formatMap)
            {
                kv.Value.Dispose();
            }
            m_formatMap.Clear();

            foreach (var kv in m_faceMap)
            {
                kv.Value.Dispose();
            }
            m_faceMap.Clear();

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

        public void Grid(uint id, RectangleF rect, Color4? fillColor, Color4? gridColor, GridInfo grid)
        {
            SolidColorBrush fillBrush = null;
            if (fillColor.HasValue)
            {
                if (!m_brushMap.TryGetValue(fillColor.Value, out fillBrush))
                {
                    fillBrush = new SolidColorBrush(m_device.D2DDeviceContext, fillColor.Value);
                    m_brushMap.Add(fillColor.Value, fillBrush);
                }
                m_device.D2DDeviceContext.FillRectangle(rect, fillBrush);
            }

            SolidColorBrush gridBrush = null;
            if (gridColor.HasValue)
            {
                if (!m_brushMap.TryGetValue(gridColor.Value, out gridBrush))
                {
                    gridBrush = new SolidColorBrush(m_device.D2DDeviceContext, gridColor.Value);
                    m_brushMap.Add(gridColor.Value, gridBrush);
                }
                for (var y = rect.Y; y <= (rect.Y + rect.Height); y += grid.CellSize)
                {
                    m_device.D2DDeviceContext.DrawLine(new Vector2(rect.X, y), new Vector2(rect.X + rect.Width, y), gridBrush, grid.LineWidth);
                }
                for (var x = rect.X; x <= (rect.X + rect.Width); x += grid.CellSize)
                {
                    m_device.D2DDeviceContext.DrawLine(new Vector2(x, rect.Y), new Vector2(x, rect.Y + rect.Height), gridBrush, grid.LineWidth);
                }
            }
        }

        public void Text(uint id, RectangleF rect, string text, Color4? color, TextInfo info)
        {
            if (!color.HasValue)
            {
                return;
            }
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var brush = GetOrCreateBrush(color.Value);

            if (true)
            {
                TextByFormat(rect, text, info, brush);
            }
            else
            {
                TextByGlyph(rect, text, info, brush);
            }
        }

        private SolidColorBrush GetOrCreateBrush(Color4 color)
        {
            SolidColorBrush brush;
            if (!m_brushMap.TryGetValue(color, out brush))
            {
                brush = new SolidColorBrush(m_device.D2DDeviceContext, color);
                m_brushMap.Add(color, brush);
            }

            return brush;
        }

        IEnumerable<int> EnumCodePoints(string input)
        {
            for (var i = 0; i < input.Length; i += char.IsSurrogatePair(input, i) ? 2 : 1)
            {
                yield return char.ConvertToUtf32(input, i);
            }
        }

        const int DWRITE_E_NOCOLORLOR = unchecked((int)0x8898500C);


        private void TextByGlyph(RectangleF rect, string text, TextInfo info, SolidColorBrush brush)
        {
            FontFace fontFace;
            if (!m_faceMap.TryGetValue(info.Font, out fontFace))
            {
                using (var f = new SharpDX.DirectWrite.Factory())
                using (var collection = f.GetSystemFontCollection(false))
                {
                    int familyIndex;
                    if (!collection.FindFamilyName(info.Font.FamilylName, out familyIndex))
                    {
                        return;
                    }

                    using (var family = collection.GetFontFamily(familyIndex))
                    using (var font = family.GetFont(0))
                    {
                        fontFace = new FontFace(font);
                        m_faceMap.Add(info.Font, fontFace);
                    }
                }
            }

            var codePoints = EnumCodePoints(text).ToArray();
            var indices = fontFace.GetGlyphIndices(codePoints);

            // Get glyph
            var metrices = fontFace.GetDesignGlyphMetrics(indices, false);

            // draw
            var glyphRun = new GlyphRun
            {
                FontFace = fontFace,
                Indices = indices,
                FontSize = info.Font.Size,
            };

            bool done = false;
            using (var f = new SharpDX.DirectWrite.Factory())
            using (var ff = f.QueryInterface<SharpDX.DirectWrite.Factory4>())
            {
                var desc = new GlyphRunDescription
                {

                };
                ColorGlyphRunEnumerator it;
                var result = ff.TryTranslateColorGlyphRun(0, 0, glyphRun,
                    null, MeasuringMode.Natural, null, 0, out it);
                if (result.Code == DWRITE_E_NOCOLORLOR)
                {
                    m_device.D2DDeviceContext.DrawGlyphRun(rect.TopLeft + new Vector2(0, info.Font.Size), glyphRun, brush, MeasuringMode.Natural);
                }
                else
                {
                    while (true)
                    {
                        var colorBrush = GetOrCreateBrush(new Color4(
                            it.CurrentRun.RunColor.R,
                            it.CurrentRun.RunColor.G,
                            it.CurrentRun.RunColor.B,
                            it.CurrentRun.RunColor.A));
                        m_device.D2DDeviceContext.DrawGlyphRun(rect.TopLeft + new Vector2(0, info.Font.Size),
                            it.CurrentRun.GlyphRun, colorBrush, MeasuringMode.Natural);
                        done = true;

                        SharpDX.Mathematics.Interop.RawBool hasNext;
                        it.MoveNext(out hasNext);
                        if (!hasNext)
                        {
                            break; ;
                        }
                    }
                }
            }
        }


        private void TextByFormat(RectangleF rect, string text, TextInfo info, SolidColorBrush brush)
        {
            TextFormat textFormat;
            if (!m_formatMap.TryGetValue(info.Font, out textFormat))
            {
                using (var f = new SharpDX.DirectWrite.Factory())
                {
                    textFormat = new TextFormat(f, info.Font.FamilylName, info.Font.Size);
                }

                switch (info.Alignment.HorizontalAlignment)
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

                switch (info.Alignment.VerticalAlignment)
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

            m_device.D2DDeviceContext.DrawText(text, textFormat, rect, brush, DrawTextOptions.EnableColorFont);
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

            //m_device.D2DDeviceContext.DrawBitmap(bitmap, new RectangleF(rect.X, rect.Y, w, h), 1.0f, BitmapInterpolationMode.Linear);
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