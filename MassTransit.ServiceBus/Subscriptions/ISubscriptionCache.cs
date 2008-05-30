namespace MassTransit.ServiceBus.Subscriptions
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Defines storage for subscriptions
	/// </summary>
	public interface ISubscriptionCache : IDisposable
	{
		IList<Subscription> List();

		/// <summary>
		/// Returns a list of endpoints that are subscribed to the specified message type
		/// </summary>
		/// <param name="messageName">Message to find the Uri's for</param>
		/// <returns>A list of endpoints subscribed to the message type</returns>
		IList<Subscription> List(string messageName);

		/// <summary>
		/// Returns a list of endpoints that are subscribed to the specified message type
		/// </summary>
		/// <param name="messageName">Message to find the Uri's for</param>
		/// <param name="correlationId">The correlation id to include in the query</param>
		/// <returns>A list of endpoints subscribed to the message type</returns>
		IList<Subscription> List(string messageName, string correlationId);

		/// <summary>
		/// Add a message type and endpointUri pair to the subscription storage
		/// </summary>
		void Add(Subscription subscription);

		/// <summary>
		/// Removes a message from the subscription store.
		/// </summary>
		void Remove(Subscription subscription);

		event EventHandler<SubscriptionEventArgs> OnAddSubscription;
		event EventHandler<SubscriptionEventArgs> OnRemoveSubscription;
	}
}