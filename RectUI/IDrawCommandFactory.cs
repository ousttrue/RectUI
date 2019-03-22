using RectUI.Graphics;
using System.Collections.Generic;


namespace RectUI
{
    /// <summary>
    /// RectRegionの内部を描画する
    /// </summary>
    public interface IDrawCommandFactory
    {
        IEnumerable<DrawCommand> GetCommands(UIContext uiContext, RectRegion r);
    }

    public class RectDrawer : IDrawCommandFactory
    {
        protected Style m_thema = Style.Default;

        public virtual IEnumerable<DrawCommand> GetCommands(UIContext uiContext, RectRegion r)
        {
            yield return new DrawCommand
            {
                DrawType = DrawType.Rectangle,
                Rectangle = new SharpDX.RectangleF(r.Rect.X, r.Rect.Y, r.Rect.Width, r.Rect.Height),
                FillColor = m_thema.GetFillColor(uiContext, r),
                BorderColor = m_thema.GetBorderColor(uiContext, r),
            };
        }
    }

    public class TextLabelDrawer : RectDrawer
    {
        public string Label;

        public override IEnumerable<DrawCommand> GetCommands(UIContext uiContext, RectRegion r)
        {
            foreach (var x in base.GetCommands(uiContext, r))
            {
                yield return x;
            }

            yield return new DrawCommand
            {
                DrawType = DrawType.Text,
                Rectangle = new SharpDX.RectangleF(r.Rect.X, r.Rect.Y, r.Rect.Width, r.Rect.Height),
                Text = Label,
                TextColor = m_thema.GetBorderColor(uiContext, r),
                Font = "Arial",
                FontSize = 18,
            };
        }
    }
}
