using RectUI.Graphics;
using System.Collections.Generic;


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
            MouseMiddleDragged += D3DRegion_MouseMiddleDragged;
            OnWheel += D3DRegion_OnWheel;
        }

        private void D3DRegion_OnWheel(RectRegion arg1, int arg2)
        {
            _camera.Dolly(arg2);
        }

        int _mx;
        int _my;
        private void D3DRegion_MouseMiddleDragged(RectRegion arg1, DragEvent arg2, int arg3, int arg4)
        {
            switch (arg2)
            {
                case DragEvent.Begin:
                    break;

                case DragEvent.Drag:
                    {
                        var dx = arg3 - _mx;
                        var dy = arg4 - _my;
                        _camera.Shift(dx, dy);
                    }
                    break;

                case DragEvent.End:
                    break;
            }

            _mx = arg3;
            _my = arg4;
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

        protected override void Layout()
        {
            _camera.Resize(Rect.Width, Rect.Height);
        }

        public override IEnumerable<D2DDrawCommand> GetDrawCommands(bool isActive, bool isHover)
        {
            _camera.Update();

            yield return new D2DDrawCommand
            {
                RegionID = ID,
                Rectangle = Rect.ToSharpDX(),
                DrawType = DrawType.Scene,
                Camera = _camera,
            };
        }
    }
}
