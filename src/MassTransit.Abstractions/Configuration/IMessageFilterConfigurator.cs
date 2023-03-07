namespace MassTransit
{
    using System;


    /// <summary>
    /// Configures a message filter, for including and excluding message types
    /// </summary>
    public interface IMessageFilterConfigurator :
        IMessageTypeFilterConfigurator
    {
        /// <summary>
        /// Include the message if it is the specified message type and matches the specified filter expression
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="filter">The filter expression</param>
        void Include<T>(Func<T, bool> filter)
            where T : class;

        /// <summary>
        /// Exclude the message if it is the specified message type and matches the specified filter expression
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="filter">The filter expression</param>
        void Exclude<T>(Func<T, bool> filter)
            where T : class;
    }
}
