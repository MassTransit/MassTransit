namespace MassTransit
{
    using System;
    using Configuration;


    /// <summary>
    /// Used to register conventions for consumer message types
    /// </summary>
    public static class ConsumerConvention
    {
        /// <summary>
        /// Register a consumer convention to be used for finding message types
        /// </summary>
        /// <typeparam name="T">The convention type</typeparam>
        public static bool Register<T>()
            where T : IConsumerConvention, new()
        {
            var convention = new T();

            return ConsumerConventionCache.TryAdd(convention);
        }

        /// <summary>
        /// Register a consumer convention to be used for finding message types
        /// </summary>
        /// <typeparam name="T">The convention type</typeparam>
        public static bool Register<T>(T convention)
            where T : IConsumerConvention
        {
            if (convention == null)
                throw new ArgumentNullException(nameof(convention));

            return ConsumerConventionCache.TryAdd(convention);
        }

        /// <summary>
        /// Remove a consumer convention used for finding message types
        /// </summary>
        /// <typeparam name="T">The convention type to remove</typeparam>
        public static void Remove<T>()
            where T : IConsumerConvention
        {
            ConsumerConventionCache.Remove<T>();
        }
    }
}
