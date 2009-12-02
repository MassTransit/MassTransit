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
	using MassTransit.Pipeline.Sinks;
	using Messages;
	using Pipeline;

	public class SagaDistributorSubscriber :
		PipelineSubscriberBase
	{
		public override IEnumerable<UnsubscribeAction> Subscribe<TComponent>(ISubscriberContext context, TComponent instance)
		{
			Type distributorInterface = typeof (TComponent).GetInterfaces()
				.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (ISagaDistributorWorker<>))
				.SingleOrDefault();

			if (distributorInterface != null)
			{
				Type sagaType = distributorInterface.GetGenericArguments().First();
			}

			yield break;
		}

		public override IEnumerable<UnsubscribeAction> Subscribe<TComponent>(ISubscriberContext context)
		{
			yield break;
		}


		protected virtual UnsubscribeAction ConnectWorker<TComponent, TMessage>(ISubscriberContext context, Consumes<Distributed<TMessage>>.Selected consumer)
			where TComponent : Consumes<Distributed<TMessage>>.Selected
			where TMessage : class
		{
			var sink = new DistributorWorkerMessageSink<Distributed<TMessage>>(message =>
				{
					// rock it
					return consumer.Accept(message) ? (Action<Distributed<TMessage>>) consumer.Consume : null;
				});

			return ConnectMessageSink(context, sink);
		}

		private UnsubscribeAction ConnectMessageSink<TMessage>(ISubscriberContext context, IPipelineSink<TMessage> sink)
			where TMessage : class
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(context.Pipeline);

			MessageRouter<TMessage> router = routerConfigurator.FindOrCreate<TMessage>();

			UnsubscribeAction result = router.Connect(sink);

			UnsubscribeAction remove = context.SubscribedTo<TMessage>();

			return () => result() && (router.SinkCount == 0) && remove();
		}
	}
}