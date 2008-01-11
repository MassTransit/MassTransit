using System;

namespace MassTransit.ServiceBus
{
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Subscriptions.Messages;

    /// <summary>
    /// Defines storage for subscriptions
    /// </summary>
    public interface ISubscriptionStorage : IDisposable
    {
        /// <summary>
        /// Returns a list of endpoints that are subscribed to the specified message type
        /// </summary>
        /// <param name="messageName">Message to find the Uri's for</param>
        /// <returns>A list of endpoints subscribed to the message type</returns>
        IList<Uri> List(string messageName);
        IList<Uri> List();

        /// <summary>
        /// Add a message type and endpoint pair to the subscription storage
        /// </summary>
        void Add(string messageName, Uri endpoint);

        /// <summary>
        /// Removes a message from the subscription store.
        /// </summary>
        void Remove(string messageName, Uri endpoint);

        event EventHandler<SubscriptionChangedEventArgs> SubscriptionChanged;
    }
}