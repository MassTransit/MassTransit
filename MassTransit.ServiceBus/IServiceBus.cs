using System;

namespace MassTransit.ServiceBus
{

	/// <summary>
	/// The base service bus interface
	/// </summary>
	public interface IServiceBus : 
        IDisposable
	{
        /// <summary>
        /// The endpoint associated with this instance
        /// </summary>
		IEndpoint Endpoint { get; }

        /// <summary>
        /// The poison endpoint associated with this instance where exception messages are sent
        /// </summary>
	    IEndpoint PoisonEndpoint { get; }

        /// <summary>
        /// Adds a message handler to the service bus for handling a specific type of message
        /// </summary>
        /// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
        /// <param name="callback">The callback to invoke when messages of the specified type arrive on the service bus</param>
	    void Subscribe<T>(MessageReceivedCallback<T> callback) where T : IMessage;

        /// <summary>
        /// Adds a message handler to the service bus for handling a specific type of message
        /// </summary>
        /// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
        /// <param name="callback">The callback to invoke when messages of the specified type arrive on the service bus</param>
        /// <param name="condition">A condition predicate to filter which messages are handled by the callback</param>
        void Subscribe<T>(MessageReceivedCallback<T> callback, Predicate<T> condition) where T : IMessage;

		/// <summary>
		/// Publishes a message to all subscribed consumers for the message type
		/// </summary>
		/// <typeparam name="T">The type of the message</typeparam>
		/// <param name="messages">The messages to be published</param>
		void Publish<T>(params T[] messages) where T : IMessage;

		/// <summary>
		/// Submits a request message to the default destination for the message type
		/// </summary>
		/// <typeparam name="T">The type of message</typeparam>
        /// <param name="destinationEndpoint">The destination for the message</param>
        /// <param name="messages">The messages to be sent</param>
		/// <returns>An IAsyncResult that can be used to wait for the response</returns>
        IServiceBusAsyncResult Request<T>(IEndpoint destinationEndpoint, params T[] messages) where T : IMessage;

		/// <summary>
		/// Sends a list of messages to the specified destination
		/// </summary>
		/// <param name="destinationEndpoint">The destination for the message</param>
		/// <param name="messages">The list of messages</param>
        void Send<T>(IEndpoint destinationEndpoint, params T[] messages) where T : IMessage;
	}
}