using RectUI.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RectUI.Application
{
    class Dispatcher
    {
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
                var call = CreateCall(mi, self);
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

        static NotifyFunc CreateCall4<S, A0, A1, A2, A3>(MethodInfo mi, S self)
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

        static NotifyFunc CreateCall6<S, A0, A1, A2, A3, A4, A5>(MethodInfo mi, S self)
        {
            var call = GenericInvokeCallFactory.BindAction<S, A0, A1, A2, A3, A4, A5>(mi, self);

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

                if (!it.MoveNext()) throw new Exception();
                var a5 = default(A5);
                it.Current.Deserialize(ref a5);

                call(a0, a1, a2, a3, a4, a5);
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

                case 6:
                    g = typeof(Dispatcher).GetMethod(nameof(CreateCall6),
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
            if (!m_map.TryGetValue(method, out func))
            {
                Console.WriteLine($"{method} not found");
                return;
            }
            func(args);
        }
    }
}
