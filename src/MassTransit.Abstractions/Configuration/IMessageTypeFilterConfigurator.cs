namespace MassTransit
{
    using System;


    /// <summary>
    /// Configures a message filter, for including and excluding message types
    /// </summary>
    public interface IMessageTypeFilterConfigurator
    {
        /// <summary>
        /// Include the message if it is any of the specified message types
        /// </summary>
        /// <param name="messageTypes"></param>
        void Include(params Type[] messageTypes);

        /// <summary>
        /// Include the type matches the specified filter expression
        /// </summary>
        /// <param name="filter">The filter expression</param>
        void Include(Func<Type, bool> filter);

        /// <summary>
        /// Include the message if it is the specified message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        void Include<T>()
            where T : class;

        /// <summary>
        /// Exclude the message if it is any of the specified message types
        /// </summary>
        /// <param name="messageTypes"></param>
        void Exclude(params Type[] messageTypes);

        /// <summary>
        /// Exclude the type matches the specified filter expression
        /// </summary>
        /// <param name="filter">The filter expression</param>
        void Exclude(Func<Type, bool> filter);

        /// <summary>
        /// Exclude the message if it is the specified message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        void Exclude<T>()
            where T : class;
    }
}
