// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit
{
    using System;
    using Magnum.Reflection;

	/// <summary>
	/// Publish extensions to <see cref="IServiceBus"/>. Contains
	/// overloads for both concrete class-publishes and publish-by-interface
	/// with anonymous dictionaries.
	/// </summary>
    public static class PublishExtensions
    {
		/// <summary>
		/// <see cref="IServiceBus.Publish{T}"/>: this is a statically
		/// typed overload for the publish on IServiceBus; one that doesn't
		/// take a contextCallback.
		/// </summary>
		/// <typeparam name="T">The message type to publish</typeparam>
		/// <param name="bus">The bus to publish on</param>
		/// <param name="message">The message to publishs</param>
        public static void Publish<T>(this IServiceBus bus, T message)
            where T : class
        {
            bus.Publish(message, x => { });
        }

		/// <summary>
		/// <see cref="IServiceBus.Publish{T}"/>: this is a "dynamically"
		/// typed overload - give it an interface as its type parameter,
		/// and a loosely typed dictionary of values and the MassTransit
		/// underlying infrastructure will populate an object instance
		/// with the passed values. It actually does this with DynamicProxy
		/// in the background.
		/// </summary>
		/// <typeparam name="T">The type of the interface or
		/// non-sealed class with all-virtual members.</typeparam>
		/// <param name="bus">The bus to publish on.</param>
		/// <param name="values">The dictionary of values to place in the
		/// object instance to implement the interface.</param>
        public static void Publish<T>(this IServiceBus bus, object values)
            where T : class
        {
            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            bus.Publish(message, x => { });
        }

		/// <summary>
		/// <see cref="Publish{T}(MassTransit.IServiceBus,object)"/>: this
		/// overload further takes an action; it allows you to set <see cref="IPublishContext"/>
		/// meta-data. Also <see cref="IServiceBus.Publish{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type of the message to publish</typeparam>
		/// <param name="bus">The bus to publish the message on.</param>
		/// <param name="values">The dictionary of values to become hydrated and
		/// published under the type of the interface.</param>
		/// <param name="contextCallback">The context callback.</param>
        public static void Publish<T>(this IServiceBus bus, object values, Action<IPublishContext<T>> contextCallback)
            where T : class
        {
            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            bus.Publish(message, contextCallback);
        }
    }
}