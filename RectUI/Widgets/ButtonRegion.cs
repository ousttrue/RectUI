using RectUI.Graphics;
using System;
using System.Collections.Generic;


namespace RectUI.Widgets
{
    public class ButtonRegion : RectRegion
    {
        public string Label
        {
            get;
            set;
        }

        Action<RectRegion> m_action;
        public Action<RectRegion> Action
        {
            set
            {
                if (m_action != null)
                {
                    MouseLeftClicked -= m_action;
                }
                m_action = value;
                if (m_action != null)
                {
                    MouseLeftClicked += m_action;
                }
            }
        }

        public ButtonRegion()
        {
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

        public override void GetDrawCommands(List<D2DDrawCommand> list, bool isActive, bool isHover)
        {
            list.Add(new D2DDrawCommand
            {
                RegionID = ID,
                Rectangle = Rect.ToSharpDX(),
                DrawType = DrawType.Rectangle,
                FillColor = GetFillColor(isActive, isHover),
                BorderColor = GetBorderColor(isActive, isHover)
            });

            list.Add(new D2DDrawCommand
            {
                RegionID = ID,
                Rectangle = Rect.ToSharpDX(),
                DrawType = DrawType.Text,
                TextColor = GetTextColor(isActive, isHover),
                Font = m_font,
                Text = new TextInfo
                {
                    Text = Label,
                    HorizontalAlignment = TextHorizontalAlignment.Center,
                    VerticalAlignment = TextVerticalAlignment.Center,
                }
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
