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
namespace MassTransit.Distributor.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Magnum.Reflection;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using MassTransit.Pipeline.Configuration.Subscribers;
	using Messages;
	using Pipeline;

	public class DistributorSubscriber :
		PipelineSubscriberBase
	{
		public override IEnumerable<UnsubscribeAction> Subscribe<TComponent>(ISubscriberContext context, TComponent instance)
		{
			Type distributorInterface = typeof (TComponent).GetInterfaces()
				.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (IDistributor<>))
				.SingleOrDefault();

			if (distributorInterface != null)
			{
				Type messageType = distributorInterface.GetGenericArguments().First();

				if (!context.HasMessageTypeBeenDefined(messageType))
				{
					context.MessageTypeWasDefined(messageType);

					yield return this.FastInvoke<DistributorSubscriber, UnsubscribeAction>(new[] {typeof (TComponent), messageType}, "ConnectDistributor", context, instance);
				}
			}

			distributorInterface = typeof (TComponent).GetInterfaces()
				.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (IWorker<>))
				.SingleOrDefault();

			if (distributorInterface != null)
			{
				Type messageType = distributorInterface.GetGenericArguments().First();

				Type distributedMessageType = typeof (Distributed<>).MakeGenericType(messageType);

				if (!context.HasMessageTypeBeenDefined(distributedMessageType))
				{
					context.MessageTypeWasDefined(distributedMessageType);

					yield return this.FastInvoke<DistributorSubscriber, UnsubscribeAction>(new[] {typeof (TComponent), messageType}, "ConnectWorker", context, instance);
				}
			}
		}

		public override IEnumerable<UnsubscribeAction> Subscribe<TComponent>(ISubscriberContext context)
		{
			yield break;
		}

		protected virtual UnsubscribeAction ConnectDistributor<TComponent, TMessage>(ISubscriberContext context, Consumes<TMessage>.Selected consumer)
			where TComponent : Consumes<TMessage>.Selected
			where TMessage : class
		{
			var sink = new DistributorMessageSink<TMessage>(message =>
				{
					// rock it
					return consumer.Accept(message) ? (Action<TMessage>) consumer.Consume : null;
				});

			return context.Pipeline.ConnectToRouter(sink, () => context.SubscribedTo<TMessage>());
		}


		protected virtual UnsubscribeAction ConnectWorker<TComponent, TMessage>(ISubscriberContext context, Consumes<Distributed<TMessage>>.Selected consumer)
			where TComponent : Consumes<Distributed<TMessage>>.Selected
			where TMessage : class
		{
			var sink = new WorkerMessageSink<Distributed<TMessage>>(message =>
				{
					// rock it
					return consumer.Accept(message) ? (Action<Distributed<TMessage>>) consumer.Consume : null;
				});

			return context.Pipeline.ConnectToRouter(sink, () => context.SubscribedTo<Distributed<TMessage>>());
		}
	}
}