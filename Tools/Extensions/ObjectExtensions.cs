using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Xml.Serialization;
using System.IO;

namespace Tools.Extensions
{
    public static class ObjectExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNull<T>(this T self) where  T : class { return (null == self); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNotNull<T>(this T self) where T : class { return (null != self); }
        public static string ToFlatString(this object source, string prefix = @"[", string postfix = @"]", string keyValueSeparator = @":", string sequenceSeparator = @"|")
        {
            return string.Concat(prefix, string.Join(sequenceSeparator, TypeDescriptor.GetProperties(source).OfType<PropertyDescriptor>().Select(x => string.Concat(x.Name, keyValueSeparator, x.GetValue(source)))), postfix);
        }
        /*
        decimal qwe = 1m;
        object foo = 123.456m;
        var bar = foo.CastTo<decimal>();
        var baz = foo.CastTo(qwe);
        */
        public static T CastTo<T>(this object self, T variable = default) { return (T)self; }
        /*
        var foo = DBNull.Value.With(w => Convert.IsDBNull(w));
        DataBinder.Eval(item.DataItem, "Foo").With(w => !Convert.IsDBNull(w)? (int)w : -1);
        */
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void With<T>(this T self, Action<T> act) { act(self); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static R With<T, R>(this T self, Func<T, R> func) { return func(self); }

        public static T DeepCopy<T>(this T self) where T : class
        {
            using (var stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, self);
                stream.Position = 0;

                return (T)serializer.Deserialize(stream);
            }
        }
    }
}
