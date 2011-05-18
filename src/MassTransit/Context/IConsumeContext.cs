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
namespace MassTransit.Context
{
	using System;

	public interface IConsumeContext<T> :
		IConsumeContext,
		IMessageContext<T>
	{
		/// <summary>
		/// Send the message to the end of the input queue so that it can be processed again later
		/// </summary>
		void RetryLater();

		/// <summary>
		/// Sends the message to either the fault address if specified or publishes the fault
		/// </summary>
		void GenerateFault(Exception ex);
	}

	/// <summary>
	/// The consumer context can be used by message consumers to retrieve out-of-band information
	/// related to a message
	/// </summary>
	public interface IConsumeContext :
		IMessageContext
	{
		/// <summary>
		/// The bus on which the message was received
		/// </summary>
		IServiceBus Bus { get; }

		/// <summary>
		/// The endpoint from which the message was received
		/// </summary>
		IEndpoint Endpoint { get; }

		/// <summary>
		/// The input address on which the message was received
		/// </summary>
		Uri InputAddress { get; }

		/// <summary>
		/// Retrieves a specified message type from the consumer context, if available
		/// </summary>
		/// <typeparam name="T">The message type requested</typeparam>
		/// <param name="context">The message context for the requested message type</param>
		/// <returns>True if the message type is available, otherwise false.</returns>
		bool TryGetContext<T>(out IConsumeContext<T> context)
			where T : class;

		/// <summary>
		/// Respond to the current message, sending directly to the ResponseAddress if specified
		/// otherwise publishing the message
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message">The message to send in response</param>
		/// <param name="contextCallback">The context action for specifying additional context information</param>
		void Respond<T>(T message, Action<ISendContext<T>> contextCallback)
			where T : class;
	}
}