using SharpDX;
using SharpDX.DXGI;
using System;


namespace Graphics
{
    public class D3D11Device : IDisposable
    {
        IntPtr _hWnd;

        public SharpDX.Direct3D11.Device Device
        {
            get;
            private set;
        }

        public SharpDX.Direct3D11.DeviceContext Context
        {
            get;
            private set;
        }

        public Device2 DXGIDevice
        {
            get;
            private set;
        }
        
        public SharpDX.Direct2D1.Device D2DDevice
        {
            get;
            private set;
        }

        public SharpDX.Direct2D1.DeviceContext D2DDeviceContext
        {
            get;
            private set;
        }

        public Size2F Dpi
        {
            get;
            private set;
        }

        public void Dispose()
        {
            _hWnd = IntPtr.Zero;

            if (D2DDeviceContext != null)
            {
                D2DDeviceContext.Dispose();
                D2DDeviceContext = null;
            }

            if (D2DDevice != null)
            {
                D2DDevice.Dispose();
                D2DDevice = null;
            }

            if (DXGIDevice != null)
            {
                DXGIDevice.Dispose();
                DXGIDevice = null;
            }

            if (Context != null)
            {
                Context.Dispose();
                Context = null;
            }
            if (Device != null)
            {
                Device.Dispose();
                Device = null;
            }
        }

        D3D11Device(SharpDX.Direct3D11.Device device)
        {
            Device = device;
            Context = Device.ImmediateContext;

            // D2D
            DXGIDevice = Device.QueryInterface<Device2>();
            D2DDevice = new SharpDX.Direct2D1.Device(DXGIDevice);
            D2DDeviceContext = new SharpDX.Direct2D1.DeviceContext(D2DDevice,
                SharpDX.Direct2D1.DeviceContextOptions.None);

            using (var factroy = new SharpDX.Direct2D1.Factory(SharpDX.Direct2D1.FactoryType.SingleThreaded))
            {
                Dpi = factroy.DesktopDpi;
            }
        }

        static public D3D11Device Create()
        {
            var flags =
                SharpDX.Direct3D11.DeviceCreationFlags.Debug
                | SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport
                ;
            var device = new SharpDX.Direct3D11.Device(
                SharpDX.Direct3D.DriverType.Hardware, flags, SharpDX.Direct3D.FeatureLevel.Level_11_1
                )
                ;

            return new D3D11Device(device);
        }

        public DXGISwapChain CreateSwapchain(IntPtr hWnd)
        {
            // SwapChain description
            var desc = new SwapChainDescription1()
            {
                Width = 0,
                Height = 0,
                Format = Format.R8G8B8A8_UNorm,
                Stereo = false,
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.BackBuffer | Usage.RenderTargetOutput,
                BufferCount = 1,
                Scaling = Scaling.Stretch,
                SwapEffect = SwapEffect.Discard,
                AlphaMode = AlphaMode.Unspecified,
                Flags = SwapChainFlags.None,
            };
            using (var factory1 = new Factory1())
            using (var factory2 = factory1.QueryInterface<Factory2>())
            {
                var swapchain = new SwapChain1(factory2, Device, hWnd, ref desc);
                return new DXGISwapChain(swapchain);
            }
        }
    }
}
