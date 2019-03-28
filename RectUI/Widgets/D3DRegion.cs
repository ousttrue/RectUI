using System.Collections.Generic;
using RectUI.Assets;
using RectUI.Graphics;

namespace RectUI.Widgets
{
    public class D3DRegion: RectRegion
    {
        public D3DRegion(Scene scene)
        {
            NormalColor = ColorKeys.ButtonNormal;
            HoverColor = ColorKeys.ButtonHover;
            ActiveColor = ColorKeys.ButtonActive;
        }

        public override IEnumerable<IEnumerable<DrawCommand>> GetDrawCommands(bool isActive, bool isHover)
        {
            return base.GetDrawCommands(isActive, isHover);
        }
    }
}
