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
                LeftClicked += m_action;
            }

            NormalColor = ColorKeys.ButtonNormal;
            HoverColor = ColorKeys.ButtonHover;
            ActiveColor = ColorKeys.ButtonActive;
        }

        public override IEnumerable<IEnumerable<DrawCommand>> GetDrawCommands(bool isActive, bool isHover)
        {
            yield return DrawCommandFactory.DrawRectCommands(Rect.ToSharpDX(),
                GetFillColor(isActive, isHover),
                GetBorderColor(isActive, isHover));

            yield return DrawCommandFactory.DrawTextCommands(this, 
                GetTextColor(isActive, isHover), "MSGothic", 18, 2, 2, 2, 2, Content as string);
        }

        public override void Dispose()
        {
            if (m_action != null)
            {
                LeftClicked -= m_action;
            }
            base.Dispose();
        }
    }
}
