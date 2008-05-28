/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

namespace MassTransit.ServiceBus
{
	using System;
	using Internal;

	/// <summary>
	/// IEndpoint is implemented by an endpoint. An endpoint is an addressable location on the network.
	/// </summary>
	public interface IEndpoint :
		IDisposable
	{
		/// <summary>
		/// The address of the endpoint, in URI format
		/// </summary>
		Uri Uri { get; }

		/// <summary>
		/// Returns an interface to send messages on this endpoint
		/// </summary>
		IMessageSender Sender { get; }

		/// <summary>
		/// Returns an interface to receive messages on this endpoint
		/// </summary>
		IMessageReceiver Receiver { get; }

		/// <summary>
		/// Sends a message to the endpoint
		/// </summary>
		/// <typeparam name="T">The type of the message to send</typeparam>
		/// <param name="message">The message to send</param>
		void Send<T>(T message) where T : class;

		/// <summary>
		/// Sends a message to the endpoint
		/// </summary>
		/// <typeparam name="T">The type of the message to send</typeparam>
		/// <param name="message">The message to send</param>
		/// <param name="timeToLive">The maximum time for the message to be received before it expires</param>
		void Send<T>(T message, TimeSpan timeToLive) where T : class;

		/// <summary>
		/// Receives any message from the endpoint
		/// </summary>
		/// <returns>The message object</returns>
		object Receive();

		/// <summary>
		/// Receives any message from the endpoint
		/// </summary>
		/// <param name="timeout">The timeout to wait for the message</param>
		/// <returns>The message object</returns>
		object Receive(TimeSpan timeout);

		/// <summary>
		/// Receives any message from the endpoint
		/// </summary>
		/// <param name="accept">A predicate to see if the message is accepted by the caller</param>
		/// <returns>The message object</returns>
		object Receive(Predicate<object> accept);

		/// <summary>
		/// Receives any message from the endpoint
		/// </summary>
		/// <param name="timeout">The timeout to wait for the message</param>
		/// <param name="accept">A predicate to see if the message is accepted by the caller</param>
		/// <returns>The message object</returns>
		object Receive(TimeSpan timeout, Predicate<object> accept);

		/// <summary>
		/// Receives a message from the endpoint
		/// </summary>
		/// <typeparam name="T">The type of message to receive</typeparam>
		/// <returns>A message read from the endpoint</returns>
		T Receive<T>() where T : class;

		/// <summary>
		/// Receives a message from the endpoint
		/// </summary>
		/// <typeparam name="T">The type of message to receive</typeparam>
		/// <param name="timeout">The timeout to wait for the message</param>
		/// <returns>A message read from the endpoint</returns>
		T Receive<T>(TimeSpan timeout) where T : class;

		/// <summary>
		/// Receives a message from the endpoint
		/// </summary>
		/// <typeparam name="T">The type of message to receive</typeparam>
		/// <param name="accept">A predicate used to determine if the message would be accepted</param>
		/// <returns>A message read from the endpoint</returns>
		T Receive<T>(Predicate<T> accept) where T : class;

		/// <summary>
		/// Receives a message from the endpoint
		/// </summary>
		/// <typeparam name="T">The type of message to receive</typeparam>
		/// <param name="timeout">The timeout to wait for the message</param>
		/// <param name="accept">A predicate used to determine if the message would be accepted</param>
		/// <returns>A message read from the endpoint</returns>
		T Receive<T>(TimeSpan timeout, Predicate<T> accept) where T : class;


        void Subscribe(IEnvelopeConsumer consumer);
	}
}