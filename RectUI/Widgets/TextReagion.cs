using RectUI.Graphics;
using SharpDX;
using NeoSmart.Unicode;

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

        public GridInfo Grid => new GridInfo
        {
            CellSize = 18.0f,
            LineWidth = 1.0f,
        };

        public Color4? GridColor;

        public override void GetDrawCommands(IDrawProcessor rpc, bool isActive, bool isHover)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                rpc.Text(ID, Rect.ToSharpDX(),
                    Text,
                    GetTextColor(isActive, isHover),
                    new TextInfo
                    {
                        Font = Style.GetFont(FontSize),
                        Grid= Grid,
                        Alignment = Alignment
                    }
                    );
            }
        }
    }
}
