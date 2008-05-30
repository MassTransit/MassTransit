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
namespace MassTransit.ServiceBus.Internal
{
	using System;

	/// <summary>
	/// A correlated message dispatcher sends a message to any attached consumers
	/// with a matching correlation identifier
	/// </summary>
	public interface IMessageDispatcher : IDisposable, Consumes<object>.Selected
	{
		/// <summary>
		/// Connects any consumers for the component to the message dispatcher
		/// </summary>
		/// <typeparam name="T">The component type</typeparam>
		/// <param name="component">The component</param>
		void Subscribe<T>(T component) where T : class;

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
		/// Removes a component from the dispatcher so that it will not be created on demand to handle messages
		/// </summary>
		/// <typeparam name="TComponent">The type of component to remove</typeparam>
		void RemoveComponent<TComponent>() where TComponent : class;

		bool Active { get; }
	}
}