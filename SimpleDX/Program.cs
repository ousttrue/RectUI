using Graphics;
using System;


namespace SimpleDX
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var window = Window.Create();
            if (window == null)
            {
                return;
            }

            window.Show();

            using (var device = D3D11Device.Create())
            {
                var sc = device.CreateSwapchain(window.WindowHandle);

                while (window.MessageLoop())
                {

                }
            }
        }
    }
}
