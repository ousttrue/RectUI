using System;
using System.Threading;


namespace DesktopDll
{
    public class MessageLoop
    {
        public static void ProcessMessage(out bool isQuit)
        {
            var msg = default(MSG);
            while (User32.PeekMessageW(ref msg, 0, 0, 0, PM.REMOVE))
            {
                if (msg.message == WM.QUIT)
                {
                    isQuit = true;
                    return;
                }
                User32.TranslateMessage(ref msg);
                User32.DispatchMessage(ref msg);
            }
            isQuit = false;
        }
    }
}
