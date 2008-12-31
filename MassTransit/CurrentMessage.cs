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
namespace MassTransit
{
	using System;
	using Exceptions;
	using Internal;

	public static class CurrentMessage
	{
		public static Uri SourceAddress
		{
			get { return BusContext.Current.InboundMessage().SourceAddress; }
		}

		public static Uri DestinationAddress
		{
			get { return BusContext.Current.InboundMessage().DestinationAddress; }
		}

		public static Uri ResponseAddress
		{
			get { return BusContext.Current.InboundMessage().ResponseAddress; }
		}

		public static Uri FaultAddress
		{
			get { return BusContext.Current.InboundMessage().FaultAddress; }
		}

		public static int RetryCount
		{
			get { return BusContext.Current.InboundMessage().RetryCount; }
		}

		public static void Respond<T>(T message) where T : class
		{
			var context = BusContext.Current.InboundMessage();

			if (context.ResponseAddress != null)
			{
				context.GetResponseEndpoint().Send(message);
			}
			else
			{
				context.Bus.Publish(message);
			}
		}

		public static void RetryLater()
		{
			var context = BusContext.Current.InboundMessage();

			if (context.Message == null)
				throw new ConventionException("RetryLater can only be called when a message is being consumed");

			context.Bus.Endpoint.Send(context.Message, x =>
				{
					x.SetSourceAddress(context.SourceAddress);
					x.SetDestinationAddress(context.DestinationAddress);
					x.SendResponseTo(context.ResponseAddress);
					x.SendFaultTo(context.FaultAddress);
					x.SetRetryCount(context.RetryCount + 1);
				});
		}
	}
}