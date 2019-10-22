namespace MassTransit.Courier
{
    using System;
    using System.Linq.Expressions;
    using System.Security.Cryptography;


    public interface Argument
    {
        /// <summary>
        /// True if the argument has a value, false if the value is null in the routing slip
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// True if the argument was present in the itinerary
        /// </summary>
        bool IsPresent { get; }
    }


    /// <summary>
    /// An argument that may have a domain-specific implementation that goes beyond simple
    /// type usage. For instance, encrypted values maybe mapped
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Argument<out T> :
        Argument
    {
        /// <summary>
        /// The argument value
        /// </summary>
        T Value { get; }
    }


    public interface ArgumentConfigurator<T, TArguments>
        where TArguments : class
    {
        void Encrypted(byte[] key, byte[] iv);
    }


    class ArgumentConfiguratorImpl<T, TArguments> :
        ArgumentConfigurator<T, TArguments>
        where TArguments : class
    {
        public void Encrypted(byte[] key, byte[] iv)
        {
            using (var x = new AesCryptoServiceProvider())
            {
                x.Key = key;
                x.IV = iv;
            }
        }
    }


    public abstract class ArgumentMap<TArguments>
        where TArguments : class
    {
        public virtual void Map<T>(Expression<Func<TArguments, T>> propertyExpression, Action<ArgumentConfigurator<T, TArguments>> configure)
        {
            var configurator = new ArgumentConfiguratorImpl<T, TArguments>();

            configure(configurator);
        }
    }
}
