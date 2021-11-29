namespace MassTransit.DependencyInjection.Registration
{
    using System;


    /// <summary>
    /// Specify the consumer, saga, and activity types to include/exclude
    /// </summary>
    public interface IRegistrationFilterConfigurator
    {
        /// <summary>
        /// Include the specified types
        /// </summary>
        /// <param name="types"></param>
        void Include(params Type[] types);

        /// <summary>
        /// Include the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Include<T>();

        /// <summary>
        /// Exclude the specified types
        /// </summary>
        /// <param name="types"></param>
        void Exclude(params Type[] types);

        /// <summary>
        /// Exclude the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Exclude<T>();
    }
}
