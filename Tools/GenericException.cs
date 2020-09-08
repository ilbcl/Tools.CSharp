using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;

namespace Tools
{
    [Serializable]
    [DataContract]
    public partial class GenericException : Exception
    {
        public GenericException() { }
        public GenericException(string message) : base(message) { }
        public GenericException(string message, Exception innerException) : base(message, innerException) { }
        protected GenericException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public override string ToString() => $@"{nameof(GenericException)}({this.Message})";
        public override string Message => this.ToString();
    }
    public static class GenericExceptionExtention
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ExceptionDispatchInfo Capture(this Exception self) => ExceptionDispatchInfo.Capture(self);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Throw(this Exception self) { ExceptionDispatchInfo.Capture(self).Throw(); }
    }
    public partial class GenericException
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static GenericException<T1> Create<T1>(T1 item1) => new GenericException<T1>(item1);
    }
    [Serializable]
    [DataContract]
    public class GenericException<T> : GenericException
    {
        [DataMember] public T Item1 { get; protected set; }
        public GenericException(T item1)
        {
            Item1 = item1;
        }
        protected GenericException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Item1 = (T)info.GetValue(nameof(Item1), typeof(T));
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Item1), Item1, typeof(T));
        }
        public override string ToString() => $@"{nameof(GenericException<T>)}<{typeof(T)}>({Item1})";
    }
    public partial class GenericException
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static GenericException<T1, T2> Create<T1, T2>(T1 item1, T2 item2) => new GenericException<T1, T2>(item1, item2);
    }
    [Serializable]
    [DataContract]
    public class GenericException<T1, T2> : GenericException<T1>
    {
        [DataMember] public T2 Item2 { get; protected set; }
        public GenericException(T1 item1, T2 item2) : base(item1)
        {
            Item2 = item2;
        }
        protected GenericException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Item2 = (T2)info.GetValue(nameof(Item2), typeof(T2));
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Item2), Item2, typeof(T2));
        }
        public override string ToString() => $@"{nameof(GenericException<T1, T2>)}<{typeof(T1)},{typeof(T2)}>({Item1},{Item2})";
    }
    public partial class GenericException
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static GenericException<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) => new GenericException<T1, T2, T3>(item1, item2, item3);
    }
    [Serializable]
    [DataContract]
    public class GenericException<T1, T2, T3> : GenericException<T1, T2>
    {
        [DataMember] public T3 Item3 { get; protected set; }
        public GenericException(T1 item1, T2 item2, T3 item3) : base(item1, item2)
        {
            Item3 = item3;
        }
        protected GenericException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Item3 = (T3)info.GetValue(nameof(Item3), typeof(T3));
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Item3), Item3, typeof(T3));
        }
        public override string ToString() => $@"{nameof(GenericException<T1, T2, T3>)}<{typeof(T1)},{typeof(T2)},{typeof(T3)}>({Item1},{Item2},{Item3})";
    }
}
