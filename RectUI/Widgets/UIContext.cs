using System;
using System.Linq;


namespace RectUI.Widgets
{
    /// <summary>
    /// 
    /// </summary>
    public class UIContext
    {
        RectRegion _mouseLeftDown;
        RectRegion _mouseRightDown;
        RectRegion _mouseMiddleDown;
        public bool IsMouseLeftDown => _mouseLeftDown != null;
        public bool IsMouseRightDown => _mouseRightDown != null;
        public bool IsMouseMiddleDown => _mouseMiddleDown != null;
        public bool IsAnyDown
        {
            get { return IsMouseLeftDown | IsMouseRightDown | IsMouseMiddleDown; }
        }
        public int MouseX;
        public int MouseY;

        /// <summary>
        /// When Mouse over
        /// </summary>
        RectRegion _hover;
        public RectRegion Hover
        {
            get { return _hover; }
            set
            {
                if (_hover == value) return;
                _hover = value;
                Updated?.Invoke();
            }
        }

        /// <summary>
        /// When Drag any
        /// </summary>
        RectRegion _active;
        public RectRegion Active
        {
            get { return _active; }
            set
            {
                if (_active == value) return;
                _active = value;
                Updated?.Invoke();
            }
        }

        public event Action Updated;

        public void MouseMove(RectRegion root, int x, int y)
        {
            MouseX = x;
            MouseY = y;
            Hover = root.MouseMove(MouseX, MouseY);

            _mouseLeftDown?.MouseLeftDrag(_mouseLeftDown, DragEvent.Drag, x, y);
            _mouseRightDown?.MouseRightDrag(_mouseRightDown, DragEvent.Drag, x, y);
            _mouseMiddleDown?.MouseMiddleDrag(_mouseMiddleDown, DragEvent.Drag, x, y);
        }

        public void MouseLeftDown(int x, int y)
        {
            if (Active == null)
            {
                Active = Hover;
            }
            _mouseLeftDown = Active.ParentPath.FirstOrDefault(r => r.RequireMouseLeftCapture);
            _mouseLeftDown?.MouseLeftDrag(_mouseLeftDown, DragEvent.Begin, x, y);
            //Console.WriteLine(nameof(MouseLeftDown));
        }

        public void MouseLeftUp(int x, int y)
        {
            _mouseLeftDown?.MouseLeftClick(_mouseLeftDown);
            _mouseLeftDown?.MouseLeftDrag(_mouseLeftDown, DragEvent.End, x, y);
            _mouseLeftDown = null;
            if (!IsAnyDown)
            {
                Active = null;
            }
        }

        public void MouseRightDown(int x, int y)
        {
            if (Active == null)
            {
                Active = Hover;
            }
            _mouseRightDown = Active.ParentPath.FirstOrDefault(r => r.RequireMouseRightCapture);
            _mouseRightDown?.MouseRightDrag(_mouseRightDown, DragEvent.Begin, x, y);
            //Console.WriteLine(nameof(MouseRightDown));
        }

        public void MouseRightUp(int x, int y)
        {
            _mouseRightDown?.MouseRightDrag(_mouseRightDown, DragEvent.End, x, y);
            _mouseRightDown = null;
            if (!IsAnyDown)
            {
                Active = null;
            }
        }

        public void MouseMiddleDown(int x, int y)
        {
            if (Active == null)
            {
                Active = Hover;
            }
            _mouseMiddleDown = Active.ParentPath.FirstOrDefault(r => r.RequireMouseMiddleCapture);
            _mouseMiddleDown?.MouseMiddleDrag(_mouseMiddleDown, DragEvent.Begin, x, y);
            //Console.WriteLine(nameof(MouseMiddleDown));
        }

        public void MouseMiddleUp(int x, int y)
        {
            _mouseMiddleDown?.MouseMiddleDrag(_mouseMiddleDown, DragEvent.End, x, y);
            _mouseMiddleDown = null;
            if (!IsAnyDown)
            {
                Active = null;
            }
        }

        public void MouseWheel(int delta)
        {
            var processed = (Active ?? Hover).ParentPath.FirstOrDefault(x => x.UseMouseWheel);
            processed?.MouseWheel(processed, delta);
        }

        public void MouseLeftDoubleClicked()
        {
            var target = (_mouseLeftDown ?? Active ?? Hover);
            target.MouseLeftDoubleClick(target);
        }
    }
}
