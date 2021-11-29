namespace MassTransit.DependencyInjection
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Bind is used to store types bound to their owner, such as an IBusControl to an IMyBus.
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The bound type</typeparam>
    public class Bind<TKey, TValue> :
        IEquatable<Bind<TKey, TValue>>
        where TValue : class
    {
        public Bind(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; }

        public bool Equals(Bind<TKey, TValue> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Bind<TKey, TValue>)obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TValue>.Default.GetHashCode(Value);
        }

        public static Bind<TKey, TValue, T> Create<T>(T value)
            where T : class
        {
            return new Bind<TKey, TValue, T>(value);
        }
    }


    public class Bind<TKey1, TKey2, TValue>
        where TValue : class
    {
        public Bind(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; }
    }


    public static class Bind<TKey>
    {
        public static Bind<TKey, TValue> Create<TValue>(TValue value)
            where TValue : class
        {
            return new Bind<TKey, TValue>(value);
        }
    }
}
