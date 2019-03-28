using System.Collections.Generic;
using RectUI.Assets;
using RectUI.Graphics;

namespace RectUI.Widgets
{
    public class D3DRegion: RectRegion
    {
        Camera _camera = new Camera();

        public D3DRegion()
        {
            NormalColor = ColorKeys.ButtonNormal;
            HoverColor = ColorKeys.ButtonHover;
            ActiveColor = ColorKeys.ButtonActive;

            MouseRightDragged += D3DRegion_MouseRightDragged;
            OnWheel += D3DRegion_OnWheel;
        }

        private void D3DRegion_OnWheel(RectRegion arg1, int arg2)
        {
            _camera.Dolly(arg2);
        }

        int _rx;
        int _ry;
        private void D3DRegion_MouseRightDragged(RectRegion arg1, DragEvent arg2, int arg3, int arg4)
        {
            switch (arg2)
            {
                case DragEvent.Begin:
                    break;

                case DragEvent.Drag:
                    {
                        var dx = arg3 - _rx;
                        var dy = arg4 - _ry;
                        _camera.YawPitch(dx, dy);
                    }
                    break;

                case DragEvent.End:
                    break;
            }

            _rx = arg3;
            _ry = arg4;
        }

        public override Rect Rect
        {
            get => base.Rect;
            set
            {
                _camera.Resize(value.Width, value.Height);
                base.Rect = value;
            }
        }

        public override IEnumerable<IEnumerable<DrawCommand>> GetDrawCommands(bool isActive, bool isHover)
        {
            _camera.Update();

            yield return DrawCommandFactory.DrawSceneCommands(Rect.ToSharpDX(), _camera);
        }
    }
}
