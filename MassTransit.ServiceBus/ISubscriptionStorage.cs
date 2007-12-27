using System;

namespace MassTransit.ServiceBus
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines storage for subscriptions
    /// </summary>
    public interface ISubscriptionStorage
    {
        /// <summary>
        /// Returns a list of endpoints that are subscribed to the specified message type
        /// </summary>
        /// <typeparam name="T">The Message Type</typeparam>
        /// <param name="messages">Optional, can be a message to find the type on</param>
        /// <returns>A list of endpoints subscribed to the message type</returns>
        IList<IEndpoint> List<T>(params T[] messages);

        /// <summary>
        /// Add a message type and endpoint pair to the subscription storage
        /// </summary>
        void Add(Type messageType, IEndpoint endpoint);

        /// <summary>
        /// Removes a message from the subscription store.
        /// </summary>
        void Remove(Type messageType, IEndpoint endpoint);
    }
}