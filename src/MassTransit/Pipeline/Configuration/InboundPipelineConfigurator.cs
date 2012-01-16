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
namespace MassTransit.Pipeline.Configuration
{
    using Sinks;
	using Util;

	/// <summary><see cref="IInboundPipelineConfigurator"/>.</summary>
	public class InboundPipelineConfigurator :
		IInboundPipelineConfigurator
	{
		readonly IServiceBus _bus;
		readonly IInboundMessagePipeline _pipeline;
		readonly RegistrationList<ISubscriptionEvent> _subscriptionEventHandlers;

		InboundPipelineConfigurator(IServiceBus bus)
		{
			_subscriptionEventHandlers = new RegistrationList<ISubscriptionEvent>();
			_bus = bus;

			var router = new MessageRouter<IConsumeContext>();

			_pipeline = new InboundMessagePipeline(router, this);
		}

		public UnregisterAction Register(ISubscriptionEvent subscriptionEventHandler)
		{
			return _subscriptionEventHandlers.Register(subscriptionEventHandler);
		}

		public IInboundMessagePipeline Pipeline
		{
			get { return _pipeline; }
		}

		public IServiceBus Bus
		{
			get { return _bus; }
		}

		public UnsubscribeAction SubscribedTo<TMessage>()
			where TMessage : class
		{
			UnsubscribeAction result = () => true;

			_subscriptionEventHandlers.Each(x => { result += x.SubscribedTo<TMessage>(); });

			return result;
		}

		public UnsubscribeAction SubscribedTo<TMessage, TKey>(TKey correlationId)
			where TMessage : class, CorrelatedBy<TKey>
		{
			UnsubscribeAction result = () => true;

			_subscriptionEventHandlers.Each(x => { result += x.SubscribedTo<TMessage, TKey>(correlationId); });

			return result;
		}

		public static IInboundMessagePipeline CreateDefault(IServiceBus bus)
		{
			return new InboundPipelineConfigurator(bus)._pipeline;
		}
	}
}