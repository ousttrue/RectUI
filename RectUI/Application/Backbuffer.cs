using DesktopDll;
using RectUI.Assets;
using RectUI.Graphics;
using RectUI.JSON;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace RectUI.Application
{
    class Dispatcher
    {
        static Dispatcher()
        {
            FormatterExtensionsSerializer.SetCustomSerializer<Color4?>((f, c) =>
            {
                if (c.HasValue)
                {
                    f.Serialize(c.Value);
                }
                else
                {
                    f.Null();
                }
            });

            GenericDeserializer<MsgPackValue, Color4?>.SetCustomDeserializer(parsed =>
            {
                if (!parsed.IsNull())
                {
                    var c = default(Color4);
                    parsed.Deserialize(ref c);
                    return c;
                }
                else
                {
                    return default(Color4?);
                }
            });
        }

        delegate void NotifyFunc(ListTreeNode<MsgPackValue> args);
        Dictionary<Utf8String, NotifyFunc> m_map = new Dictionary<Utf8String, NotifyFunc>();

        public void RegisterInterface(Type methods, object self)
        {
            if (!methods.IsInterface)
            {
                throw new Exception();
            }

            foreach (var mi in methods.GetMethods())
            {
                var call=CreateCall(mi, self);
                m_map.Add(Utf8String.From(mi.Name), call);
            }
        }

        static NotifyFunc CreateCall3<S, A0, A1, A2>(MethodInfo mi, S self)
        {
            var call = GenericInvokeCallFactory.BindAction<S, A0, A1, A2>(mi, self);

            return args =>
            {
                var it = args.ArrayItems().GetEnumerator();

                if (!it.MoveNext()) throw new Exception();
                var a0 = default(A0);
                it.Current.Deserialize(ref a0);

                if (!it.MoveNext()) throw new Exception();
                var a1 = default(A1);
                it.Current.Deserialize(ref a1);

                if (!it.MoveNext()) throw new Exception();
                var a2 = default(A2);
                it.Current.Deserialize(ref a2);

                call(a0, a1, a2);
            };
        }

        static NotifyFunc CreateCall4<S, A0, A1, A2, A3>(MethodInfo mi , S self)
        {
            var call = GenericInvokeCallFactory.BindAction<S, A0, A1, A2, A3>(mi, self);

            return args =>
            {
                var it = args.ArrayItems().GetEnumerator();

                if (!it.MoveNext()) throw new Exception();
                var a0 = default(A0);
                it.Current.Deserialize(ref a0);

                if (!it.MoveNext()) throw new Exception();
                var a1 = default(A1);
                it.Current.Deserialize(ref a1);

                if (!it.MoveNext()) throw new Exception();
                var a2 = default(A2);
                it.Current.Deserialize(ref a2);

                if (!it.MoveNext()) throw new Exception();
                var a3 = default(A3);
                it.Current.Deserialize(ref a3);

                call(a0, a1, a2, a3);
            };
        }

        static NotifyFunc CreateCall5<S, A0, A1, A2, A3, A4>(MethodInfo mi, S self)
        {
            var call = GenericInvokeCallFactory.BindAction<S, A0, A1, A2, A3, A4>(mi, self);

            return args =>
            {
                var it = args.ArrayItems().GetEnumerator();

                if (!it.MoveNext()) throw new Exception();
                var a0 = default(A0);
                it.Current.Deserialize(ref a0);

                if (!it.MoveNext()) throw new Exception();
                var a1 = default(A1);
                it.Current.Deserialize(ref a1);

                if (!it.MoveNext()) throw new Exception();
                var a2 = default(A2);
                it.Current.Deserialize(ref a2);

                if (!it.MoveNext()) throw new Exception();
                var a3 = default(A3);
                it.Current.Deserialize(ref a3);

                if (!it.MoveNext()) throw new Exception();
                var a4 = default(A4);
                it.Current.Deserialize(ref a4);

                call(a0, a1, a2, a3, a4);
            };
        }

        static NotifyFunc CreateCall(MethodInfo mi, object self)
        {
            var paramTypes = mi.GetParameters().Select(x => x.ParameterType).ToArray();

            MethodInfo g = null;
            switch (paramTypes.Length)
            {
                case 3:
                    g = typeof(Dispatcher).GetMethod(nameof(CreateCall3),
                        BindingFlags.Static | BindingFlags.NonPublic);
                    break;

                case 4:
                    g = typeof(Dispatcher).GetMethod(nameof(CreateCall4),
                        BindingFlags.Static | BindingFlags.NonPublic);
                    break;

                case 5:
                    g = typeof(Dispatcher).GetMethod(nameof(CreateCall5),
                        BindingFlags.Static | BindingFlags.NonPublic);
                    break;

                default:
                    throw new NotImplementedException();
            }

            var gg = g.MakeGenericMethod(new[] { mi.DeclaringType }.Concat(paramTypes).ToArray());
            return (NotifyFunc)gg.Invoke(null, new object[] { mi, self });
        }

        public void Dispatch(ListTreeNode<MsgPackValue> parsed)
        {
            if (!parsed.IsArray())
            {
                return;
            }

            if (parsed.GetArrayCount() != 3)
            {
                return;
            }

            if (parsed[0].GetInt32() != MsgPackFormatter.NOTIFY_TYPE)
            {
                return;
            }

            Notify(parsed[1].GetUtf8String(), parsed[2]);
        }

        void Notify(Utf8String method, ListTreeNode<MsgPackValue> args)
        {
            NotifyFunc func;
            if(!m_map.TryGetValue(method, out func))
            {
                Console.WriteLine($"{method} not found");
                return;
            }
            func(args);
        }
    }

    public class Backbuffer : IDisposable
    {
        DXGISwapChain m_swapchain;
        D2D1Bitmap m_backbuffer;
        Color4 m_clear = new Color4(0.1f, 0.2f, 0.1f, 1.0f);
        D3D11RenderTarget m_renderTarget;

        Dispatcher m_dispatcher = new Dispatcher();

        public void Dispose()
        {
            if (m_renderTarget != null)
            {
                m_renderTarget.Dispose();
                m_renderTarget = null;
            }

            if (m_backbuffer != null)
            {
                m_backbuffer.Dispose();
                m_backbuffer = null;
            }

            if (m_swapchain != null)
            {
                m_swapchain.Dispose();
                m_swapchain = null;
            }
        }

        public Backbuffer(D3D11Device device, Window window)
        {
            m_swapchain = device.CreateSwapchain(window.WindowHandle);
            m_backbuffer = m_swapchain.CreateBitmap();

            m_dispatcher.RegisterInterface(typeof(IDrawProcessor), m_backbuffer);
        }

        public void Resize(int w, int h)
        {
            m_backbuffer.Dispose();
            m_swapchain.Resize(w, h);
        }

        RectangleF m_rect;
        D3D11RenderTarget GetOrRenderTarget(D3D11Device device, uint id, RectangleF rect)
        {
            if (rect != m_rect)
            {
                m_rect = rect;
                if (m_renderTarget != null)
                {
                    m_renderTarget.Dispose();
                    m_renderTarget = null;
                }
            }
            if (m_renderTarget == null)
            {
                m_renderTarget = D3D11RenderTarget.Create(device, (int)rect.Width, (int)rect.Height);
            }
            return m_renderTarget;
        }

        static ArraySegment<byte> Skip(ArraySegment<byte> src, int skip)
        {
            return new ArraySegment<byte>(src.Array, src.Offset + skip, src.Count - skip);
        }

        public void ExecuteCommands(D3D11Device device,
            Scene scene,
            ArraySegment<byte> commands)
        {
            m_backbuffer.Device = device;
            m_backbuffer.Begin(m_clear);
            while (commands.Count > 0)
            {
                try
                {
                    var parsed = MsgPackParser.Parse(commands);

                    m_dispatcher.Dispatch(parsed);

                    commands = Skip(commands, parsed.Value.Bytes.Count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    break;
                }
            }
            m_backbuffer.End();
            m_swapchain.Present();
        }
    }
}
