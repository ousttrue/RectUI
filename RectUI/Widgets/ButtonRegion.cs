using RectUI.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RectUI.Widgets
{
    public class ButtonRegion : RectRegion
    {
        Action<RectRegion> m_action;

        public ButtonRegion(Action<RectRegion> action=null)
        {
            m_action = action;
            if (m_action != null)
            {
                MouseLeftClicked += m_action;
            }

            NormalColor = ColorKeys.ButtonNormal;
            HoverColor = ColorKeys.ButtonHover;
            ActiveColor = ColorKeys.ButtonActive;
        }

        Padding m_padding = new Padding
        {
            Left = 2,
            Top = 2,
            Right = 2,
            Bottom = 2,
        };

        FontInfo m_font = new FontInfo
        {
            Font = "MSGothic",
            Size = 18,
        };

        public override IEnumerable<IEnumerable<DrawCommand>> GetDrawCommands(bool isActive, bool isHover)
        {
            yield return DrawCommandFactory.DrawRectCommands(Rect.ToSharpDX(),
                GetFillColor(isActive, isHover),
                GetBorderColor(isActive, isHover));

            yield return DrawCommandFactory.DrawTextCommands(Rect.ToSharpDX(), m_padding,
                GetTextColor(isActive, isHover), m_font, 
                new TextInfo
                {
                    Text = Content as string,
                    HorizontalAlignment = TextHorizontalAlignment.Center,
                    VerticalAlignment = TextVerticalAlignment.Center,                   
                });
        }

        public override void Dispose()
        {
            if (m_action != null)
            {
                MouseLeftClicked -= m_action;
            }
            base.Dispose();
        }
    }
}
