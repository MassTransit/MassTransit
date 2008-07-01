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
		void Subscribe<T>(Action<IMessageContext<T>> callback) where T : class;

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="callback">The callback to invoke when messages of the specified type arrive on the service bus</param>
		/// <param name="condition">A condition predicate to filter which messages are handled by the callback</param>
		void Subscribe<T>(Action<IMessageContext<T>> callback, Predicate<T> condition) where T : class;

		/// <summary>
		/// Connects any consumers for the component to the message dispatcher
		/// </summary>
		/// <typeparam name="T">The component type</typeparam>
		/// <param name="component">The component</param>
		void Subscribe<T>(T component) where T : class;

		/// <summary>
		/// Removes a message handler from the service bus
		/// </summary>
		/// <typeparam name="T">The message type handled by the handler</typeparam>
		/// <param name="callback">The callback to remove</param>
		void Unsubscribe<T>(Action<IMessageContext<T>> callback) where T : class;

		/// <summary>
		/// Removes a message handler from the service bus
		/// </summary>
		/// <typeparam name="T">The message type handled by the handler</typeparam>
		/// <param name="callback">The callback to remove</param>
		/// <param name="condition">A condition predicate to filter which messages are handled by the callback</param>
		void Unsubscribe<T>(Action<IMessageContext<T>> callback, Predicate<T> condition) where T : class;

		/// <summary>
		/// Disconnects any consumers for the component from the message dispatcher
		/// </summary>
		/// <typeparam name="T">The component type</typeparam>
		/// <param name="component">The component</param>
		void Unsubscribe<T>(T component) where T : class;

		/// <summary>
		/// Adds a component to the dispatcher that will be created on demand to handle messages
		/// </summary>
		/// <typeparam name="TComponent">The type of the component to add</typeparam>
		void AddComponent<TComponent>() where TComponent : class;

		/// <summary>
		/// Adds a component to the dispatcher that will be created on demand to handle messages
		/// </summary>
		/// <typeparam name="TComponent">The type of the component to add</typeparam>
		void RemoveComponent<TComponent>() where TComponent : class;

		/// <summary>
		/// Binds message publishers to the all of the Produces interfaces in the component
		/// to allow publishing directly from the component without knowledge of the IServiceBus
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="producer"></param>
		void AttachProducers<T>(T producer) where T : class;

		/// <summary>
		/// Publishes a message to all subscribed consumers for the message type
		/// </summary>
		/// <typeparam name="T">The type of the message</typeparam>
		/// <param name="message">The messages to be published</param>
		void Publish<T>(T message) where T : class;

		/// <summary>
		/// Returns a request builder for this service bus to handle a request/response
		/// </summary>
		/// <returns>A request builder</returns>
		RequestBuilder Request();
	}
}