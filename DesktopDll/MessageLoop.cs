using System;
using System.Threading;


namespace DesktopDll
{
    public class MessageLoop
    {
        const int MS_PER_FRAME = 30;
        public static void Run(Action onFrame)
        {
            if (onFrame == null)
            {
                onFrame = () => { };
            }

            uint last = Winmm.timeGetTime();
            while (true)
            {
                var msg = default(MSG);
                while (User32.PeekMessageW(ref msg, 0, 0, 0, PM.REMOVE))
                {
                    if (msg.message == WM.QUIT)
                    {
                        return;
                    }
                    User32.TranslateMessage(ref msg);
                    User32.DispatchMessage(ref msg);
                }

                var now = Winmm.timeGetTime();
                var delta = (int)(now - last);
                if (delta > MS_PER_FRAME)
                {
                    last = now;
                    onFrame();
                }
                else
                {
                    var sleep = MS_PER_FRAME - delta;
                    if (sleep > 0)
                    {
                        Thread.Sleep(sleep);
                    }
                }
            }
        }

    }
}
