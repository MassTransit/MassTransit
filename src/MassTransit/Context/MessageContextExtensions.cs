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
	using Util;

	public static class MessageContextExtensions
	{
		public static void SetSourceAddress<T>(this ISendContext<T> context, string uriString)
			where T : class
		{
			context.SetSourceAddress(uriString.ToUri());
		}

		public static void SetDestinationAddress<T>(this ISendContext<T> context, string uriString)
			where T : class
		{
			context.SetDestinationAddress(uriString.ToUri());
		}

		public static void SetResponseAddress<T>(this ISendContext<T> context, string uriString)
			where T : class
		{
			context.SetResponseAddress(uriString.ToUri());
		}

		public static void SendResponseTo<T>(this ISendContext<T> context, IServiceBus bus)
			where T : class
		{
			context.SetResponseAddress(bus.Endpoint.Address.Uri);
		}

		public static void SendResponseTo<T>(this ISendContext<T> context, IEndpoint endpoint)
			where T : class
		{
			context.SetResponseAddress(endpoint.Address.Uri);
		}

		public static void SendResponseTo<T>(this ISendContext<T> context, Uri uri)
			where T : class
		{
			context.SetResponseAddress(uri);
		}

		public static void SetFaultAddress<T>(this ISendContext<T> context, string uriString)
			where T : class
		{
			context.SetFaultAddress(uriString.ToUri());
		}

		public static void SendFaultTo<T>(this ISendContext<T> context, IServiceBus bus)
			where T : class
		{
			context.SetFaultAddress(bus.Endpoint.Address.Uri);
		}

		public static void SendFaultTo<T>(this ISendContext<T> context, IEndpoint endpoint)
			where T : class
		{
			context.SetFaultAddress(endpoint.Address.Uri);
		}

		public static void SendFaultTo<T>(this ISendContext<T> context, Uri uri)
			where T : class
		{
			context.SetFaultAddress(uri);
		}

		public static void ExpiresAt<T>(this ISendContext<T> context, DateTime value)
			where T : class
		{
			context.SetExpirationTime(value);
		}

		public static void SetMessageType<T>(this ISendContext<T> context, Type messageType)
			where T : class
		{
			context.SetMessageType(messageType.ToMessageName());
		}

		public static void SetInputAddress(this IReceiveContext context, IEndpointAddress address)
		{
			context.SetInputAddress(address.Uri);
		}

		public static void Respond<T>(this IConsumeContext context, T message)
			where T : class
		{
			context.Respond(message, x => { });
		}
	}
}