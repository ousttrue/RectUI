using RectUI.Graphics;

namespace RectUI.Widgets
{
    public class TextRegion : RectRegion
    {
        string m_label;
        public string Text
        {
            get { return m_label; }
            set
            {
                if (m_label == value) return;
                m_label = value;
                Invalidate();
            }
        }

        public override void GetDrawCommands(IDrawProcessor rpc, bool isActive, bool isHover)
        {
            rpc.Rectangle(ID, Rect.ToSharpDX(),
                GetFillColor(isActive, isHover),
                GetBorderColor(isActive, isHover));

            if (!string.IsNullOrEmpty(Text))
            {
                rpc.Text(ID, Rect.ToSharpDX(),
                    GetTextColor(isActive, isHover),
                    Style.GetFont(FontSize),
                    Text,
                    Alignment
                    );
            }
        }
    }
}
