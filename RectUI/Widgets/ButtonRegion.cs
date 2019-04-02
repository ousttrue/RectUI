﻿using RectUI.Graphics;
using System;


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
            Font = FontFaceName.MSGothic,
            Size = 18,
        };

        public override void GetDrawCommands(IDrawProcessor rpc, bool isActive, bool isHover)
        {
            rpc.Rectangle(ID,Rect.ToSharpDX(),
                GetFillColor(isActive, isHover),
                GetBorderColor(isActive, isHover));

            if (!string.IsNullOrEmpty(Label))
            {
                rpc.Text(ID, Rect.ToSharpDX(),
                    GetTextColor(isActive, isHover),
                    m_font,
                    Label,
                    new TextAlignment
                    {
                        HorizontalAlignment = TextHorizontalAlignment.Center,
                        VerticalAlignment = TextVerticalAlignment.Center,
                    });
            }
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
