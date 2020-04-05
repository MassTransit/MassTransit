namespace MassTransit
{
    using System;
    using ConsumeConnectors;


    /// <summary>
    /// Used to register conventions for consumer message types
    /// </summary>
    public static class ConsumerConvention
    {
        /// <summary>
        /// Register a consumer convention to be used for finding message types
        /// </summary>
        /// <typeparam name="T">The convention type</typeparam>
        public static void Register<T>()
            where T : IConsumerConvention, new()
        {
            var convention = new T();

            ConsumerConventionCache.Add(convention);
        }

        /// <summary>
        /// Register a consumer convention to be used for finding message types
        /// </summary>
        /// <typeparam name="T">The convention type</typeparam>
        public static void Register<T>(T convention)
            where T : IConsumerConvention
        {
            if (convention == null)
                throw new ArgumentNullException(nameof(convention));

            ConsumerConventionCache.Add(convention);
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
