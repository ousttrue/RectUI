namespace RectUI
{
    /// <summary>
    /// RectRegionの内部を描画する
    /// </summary>
    public interface IRectDrawer
    {
        void Draw(D3D11Device device, D2D1Bitmap bitmap, UIContext uiContext, RectRegion r);
    }

    public class RectDrawer : IRectDrawer
    {
        Thema m_thema = Thema.Default;
            
        public void Draw(D3D11Device device, D2D1Bitmap bitmap, UIContext uiContext, RectRegion r)
        {
            bitmap.DrawRect(device, r.Rect.X, r.Rect.Y, r.Rect.Width, r.Rect.Height,
                m_thema.GetFillColor(uiContext, r),
                m_thema.GetBorderColor(uiContext, r));
        }
    }
}
