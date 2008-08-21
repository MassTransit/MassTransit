// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;
	using Subscriptions;

	/// <summary>
	/// Encapsulates the information required to publish a message on the service bus
	/// </summary>
	public interface IPublicationTypeInfo
	{
		/// <summary>
		/// Returns the time to live for the message type
		/// </summary>
		TimeSpan TimeToLive { get; }

		/// <summary>
		/// Returns the consumers for the message type being published
		/// </summary>
		/// <typeparam name="T">The type of message to be published</typeparam>
		/// <param name="context">The dispatch context from which the message is being published</param>
		/// <param name="message">The message to be published</param>
		/// <returns>A list of subscribers that are interested in the message</returns>
		IList<Subscription> GetConsumers<T>(IDispatcherContext context, T message) where T : class;

		/// <summary>
		/// Publishes a fault message to any interested consumers
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="bus"></param>
		/// <param name="ex"></param>
		/// <param name="message"></param>
		void PublishFault<T>(IServiceBus bus, Exception ex, T message) where T : class;
	}
}