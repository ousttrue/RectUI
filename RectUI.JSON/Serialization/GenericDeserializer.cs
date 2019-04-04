using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace RectUI.JSON
{
    public static class GenericDeserializer<T, U>
        where T : IListTreeItem, IValue<T>
    {
        public static V[] GenericArrayDeserializer<V>(ListTreeNode<T> s)
        {
            if (!s.IsArray())
            {
                throw new ArgumentException("not array: " + s.Value.ValueType);
            }
            var u = new V[s.GetArrayCount()];
            int i = 0;
            foreach (var x in s.ArrayItems())
            {
                x.Deserialize(ref u[i++]);
            }
            return u;
        }

        public static List<V> GenericListDeserializer<V>(ListTreeNode<T> s)
        {
            if (!s.IsArray())
            {
                throw new ArgumentException("not array: " + s.Value.ValueType);
            }
            var u = new List<V>(s.GetArrayCount());
            foreach (var x in s.ArrayItems())
            {
                var e = default(V);
                x.Deserialize(ref e);
                u.Add(e);
            }
            return u;
        }

        public static Nullable<V> GenericNullableDeserializer<V>(ListTreeNode<T> parsed) where V : struct
        {
            if (!parsed.IsNull())
            {
                var c = default(V);
                parsed.Deserialize(ref c);
                return c;
            }
            else
            {
                return default(V);
            }
        }

        public static object DefaultDictionaryDeserializer(ListTreeNode<T> s)
        {
            switch (s.Value.ValueType)
            {
                case ValueNodeType.Object:
                    {
                        var u = new Dictionary<string, object>();
                        foreach (var kv in s.ObjectItems())
                        {
                            //var e = default(object);
                            //kv.Value.Deserialize(ref e);
                            u.Add(kv.Key.GetString(), DefaultDictionaryDeserializer(kv.Value));
                        }
                        return u;
                    }

                case ValueNodeType.Null:
                    return null;

                case ValueNodeType.Boolean:
                    return s.GetBoolean();

                case ValueNodeType.Integer:
                    return s.GetInt32();

                case ValueNodeType.Number:
                    return s.GetDouble();

                case ValueNodeType.String:
                    return s.GetString();

                default:
                    throw new NotImplementedException(s.Value.ValueType.ToString());
            }
        }

        /// <summary>
        /// Deserialize Dictionary only string key
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Dictionary<string, V> DictionaryDeserializer<V>(ListTreeNode<T> s)
        {
            var d = new Dictionary<string, V>();
            foreach (var kv in s.ObjectItems())
            {
                var value = default(V);
                GenericDeserializer<T, V>.Deserialize(kv.Value, ref value);
                d.Add(kv.Key.GetString(), value);
            }
            return d;
        }

        delegate void FieldSetter(ListTreeNode<T> s, object o);
        static FieldSetter GetFieldDeserializer<V>(FieldInfo fi)
        {
            return (s, o) =>
            {
                var u = default(V);
                s.Deserialize(ref u);
                fi.SetValue(o, u);
            };
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="x"></param>
        /// <returns></returns>
        static bool IsPrimitiveDeserializer<U>(MethodInfo x)
        {
            var u = typeof(U);
            if (x.Name != $"Get{u.Name}")
            {
                /// 名前が違う
                return false;
            }

            var parameters = x.GetParameters();
            if (parameters.Length != 0)
            {
                /// 引数がある
                return false;
            }

            if (x.ReturnType != typeof(U))
            {
                /// 返り値がU型でない
                return false;
            }

            return true;
        }

        static Func<ListTreeNode<T>, U> GetDeserializer()
        {
            // primitive
            {
                var mi = typeof(T).GetMethods()
                    .Where(x => x.Name.StartsWith("Get"))
                    .FirstOrDefault(IsPrimitiveDeserializer<U>)
                    ;
                if (mi != null)
                {
                    var getter = GenericInvokeCallFactory.OpenFunc<T, U>(mi);
                    return s => getter(s.Value);
                }
            }

            var target = typeof(U);

            if (target.IsArray)
            {
                var mi = typeof(GenericDeserializer<T, U>).GetMethod("GenericArrayDeserializer",
                    BindingFlags.Static | BindingFlags.Public);
                var g = mi.MakeGenericMethod(target.GetElementType());
                return GenericInvokeCallFactory.StaticFunc<ListTreeNode<T>, U>(g);
            }

            if (target.IsGenericType)
            {
                if (target.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var mi = typeof(GenericDeserializer<T, U>).GetMethod("GenericListDeserializer",
                        BindingFlags.Static | BindingFlags.Public);
                    var g = mi.MakeGenericMethod(target.GetGenericArguments());
                    return GenericInvokeCallFactory.StaticFunc<ListTreeNode<T>, U>(g);
                }

                if (target == typeof(Dictionary<string, object>))
                {

                    var mi = typeof(GenericDeserializer<T, U>).GetMethod("DefaultDictionaryDeserializer",
                    BindingFlags.Static | BindingFlags.Public);
                    return GenericInvokeCallFactory.StaticFunc<ListTreeNode<T>, U>(mi);
                }
                else
                if (target.GetGenericTypeDefinition() == typeof(Dictionary<,>) &&
                    target.GetGenericArguments()[0] == typeof(string))
                {
                    var mi = typeof(GenericDeserializer<T, U>).GetMethod("DictionaryDeserializer",
                    BindingFlags.Static | BindingFlags.Public);
                    var g = mi.MakeGenericMethod(target.GetGenericArguments()[1]);
                    return GenericInvokeCallFactory.StaticFunc<ListTreeNode<T>, U>(g);
                }

                if (target.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var mi = typeof(GenericDeserializer<T, U>).GetMethod("GenericNullableDeserializer",
                        BindingFlags.Static | BindingFlags.Public);
                    var g = mi.MakeGenericMethod(target.GetGenericArguments());
                    return GenericInvokeCallFactory.StaticFunc<ListTreeNode<T>, U>(g);
                }
            }

            if (target.IsEnum)
            {
                var value = Expression.Parameter(typeof(int), "value");
                var cast = Expression.Convert(value, target);
                var func = Expression.Lambda(cast, value);
                var compiled = (Func<int, U>)func.Compile();
                return s =>
                {
                    if (s.IsString())
                    {
                        return (U)Enum.Parse(typeof(U), s.GetString(), true);
                    }
                    else
                    {
                        return compiled(s.GetInt32());
                    }
                };
            }

            // reflection
            {
                var fields = target.GetFields(BindingFlags.Instance | BindingFlags.Public);
                var fieldDeserializers = fields.ToDictionary(x => Utf8String.From(x.Name), x =>
                {
                    var mi = typeof(GenericDeserializer<T, U>).GetMethod("GetFieldDeserializer",
                        BindingFlags.Static | BindingFlags.NonPublic);
                    var g = mi.MakeGenericMethod(x.FieldType);
                    return (FieldSetter)g.Invoke(null, new object[] { x });
                });

                return s =>
                {
                    if (!s.IsMap())
                    {
                        throw new ArgumentException(s.Value.ValueType.ToString());
                    }

                    var t = (object)default(GenericConstructor<T, U>).Create(s);
                    foreach (var kv in s.ObjectItems())
                    {
                        FieldSetter setter;
                        if (fieldDeserializers.TryGetValue(kv.Key.GetUtf8String(), out setter))
                        {
                            setter(kv.Value, t);
                        }
                    }
                    return (U)t;
                };
            }
        }

        public delegate U Deserializer(ListTreeNode<T> node);

        public static Deserializer s_deserializer;

        public static void Deserialize(ListTreeNode<T> node, ref U value)
        {
            if (s_deserializer == null)
            {
                var d = GetDeserializer();
                s_deserializer = new Deserializer(d);
            }
            value = s_deserializer(node);
        }

        public static void SetCustomDeserializer(Deserializer deserializer)
        {
            s_deserializer = deserializer;
        }
    }
}
