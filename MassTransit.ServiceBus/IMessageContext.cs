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
	public interface IMessageContext<T>
	{
		/// <summary>
		/// The actual message being delivered
		/// </summary>
		T Message { get; }

		/// <summary>
		/// The service bus on which the message was received
		/// </summary>
		IServiceBus Bus { get; }

		/// <summary>
		/// Builds an envelope with the correlation id set to the id of the incoming envelope
		/// </summary>
		/// <param name="message">The messages to include with the reply</param>
		void Reply(object message);

		/// <summary>
		/// Moves the specified messages to the back of the list of available 
		/// messages so they can be handled later. Could screw up message order.
		/// </summary>
		void HandleMessageLater(object message);

		/// <summary>
		/// Marks the whole context as poison
		/// </summary>
		void MarkPoison();

		/// <summary>
		/// Marks a specific message as poison
		/// </summary>
		void MarkPoison(object message);
	}
}