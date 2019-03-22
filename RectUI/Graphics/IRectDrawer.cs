namespace RectUI.Graphics
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
        protected Style m_thema = Style.Default;

        public virtual void Draw(D3D11Device device, D2D1Bitmap bitmap, UIContext uiContext, RectRegion r)
        {
            bitmap.DrawRect(device, r.Rect,
                m_thema.GetFillColor(uiContext, r),
                m_thema.GetBorderColor(uiContext, r));
        }
    }

    public class TextLabelDrawer : RectDrawer
    {
        public string Label;
        public override void Draw(D3D11Device device, D2D1Bitmap bitmap, UIContext uiContext, RectRegion r)
        {
            base.Draw(device, bitmap, uiContext, r);

            bitmap.DrawText(device, r.Rect, m_thema.GetBorderColor(uiContext, r), Label);
        }
    }
}
