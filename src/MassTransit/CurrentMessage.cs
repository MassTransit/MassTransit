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
	using Context;

	public static class CurrentMessage
	{
		/// <summary>
		/// Respond to the current inbound message with either a send to the ResponseAddress or a
		/// Publish on the bus that received the message
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message">The message to send/publish</param>
		public static void Respond<T>(T message) where T : class
		{
			ContextStorage.Context(x =>
				{
					if (x.ResponseAddress != null)
					{
						x.GetResponseEndpoint()
							.Send(message, context => context.SetSourceAddress(x.Bus.Endpoint.Uri));
					}
					else
					{
						x.Bus.Publish(message);
					}
				});
		}



		/// <summary>
		/// Send a fault to either via publishing or to the Fault Endpoint
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message">The message to send/publish</param>
		public static void GenerateFault<T>(T message)
			where T : class
		{
			ContextStorage.Context(x =>
				{
					if (x.ResponseAddress != null)
					{
						x.GetFaultEndpoint()
							.Send(message, context => context.SetSourceAddress(x.Bus.Endpoint.Uri));
					}
					else
					{
						x.Bus.Publish(message);
					}
				});
		}

		/// <summary>
		/// Respond to the current inbound message with either a send to the ResponseAddress or a
		/// Publish on the bus that received the message
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message">The message to send/publish</param>
		/// <param name="contextAction">The action to setup the context on the outbound message</param>
		public static void Respond<T>(T message, Action<ISendContext<T>> contextAction)
			where T : class
		{
			ContextStorage.Context(context =>
				{
					if (context.ResponseAddress != null)
					{
						context.GetResponseEndpoint().Send(message, x =>
							{
								x.SetSourceAddress(context.Bus.Endpoint.Uri);
								contextAction(x);
							});
					}
					else
					{
						context.Bus.Publish(message, contextAction);
					}
				});
		}

		/// <summary>
		/// Puts the message back on the input queue so that it can be processed again later
		/// </summary>
		public static void RetryLater()
		{
			IConsumeContext context = ContextStorage.Context();
			if (context == null)
				throw new InvalidOperationException("No current message context was found");

			context.RetryLater();
		}

		static IEndpoint GetFaultEndpoint(this IConsumeContext context)
		{
			if (context.FaultAddress == null)
				throw new InvalidOperationException("No fault address was contained in the message");

			return context.Bus.GetEndpoint(context.FaultAddress);
		}

		static IEndpoint GetResponseEndpoint(this IConsumeContext context)
		{
			if (context.ResponseAddress == null)
				throw new InvalidOperationException("No response address was contained in the message");

			return context.Bus.GetEndpoint(context.ResponseAddress);
		}
	}
}