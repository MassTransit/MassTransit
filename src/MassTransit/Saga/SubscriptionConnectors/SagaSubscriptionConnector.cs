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
namespace MassTransit.Saga.SubscriptionConnectors
{
	using System;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using MassTransit.Pipeline.Sinks;

	public interface SagaSubscriptionConnector :
		SagaConnector
	{
		Type MessageType { get; }
	}

	public abstract class SagaSubscriptionConnector<TSaga, TMessage> :
		SagaSubscriptionConnector
		where TSaga : class, ISaga, Consumes<TMessage>.All
		where TMessage : class
	{
		readonly ISagaPolicy<TSaga, TMessage> _policy;
		readonly ISagaRepository<TSaga> _sagaRepository;

		protected SagaSubscriptionConnector(ISagaRepository<TSaga> sagaRepository,
		                                    ISagaPolicy<TSaga, TMessage> policy)
		{
			_sagaRepository = sagaRepository;
			_policy = policy;
		}

		public Type MessageType
		{
			get { return typeof (TMessage); }
		}

		public Type SagaType
		{
			get { return typeof (TSaga); }
		}

		public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator)
		{
			InboundMessageRouterConfigurator routerConfigurator = new InboundMessageRouterConfigurator(configurator.Pipeline);

			MessageRouter<IConsumeContext<TMessage>> router = routerConfigurator.FindOrCreate<TMessage>();

			ISagaMessageSink<TSaga, TMessage> sink = CreateSink();

			UnsubscribeAction result = router.Connect(sink);

			UnsubscribeAction remove = configurator.SubscribedTo<TMessage>();

			return () => result() && (router.SinkCount == 0) && remove();
		}

		public ISagaMessageSink<TSaga, TMessage> CreateSink()
		{
			return CreateSink(_sagaRepository, _policy);
		}

		protected abstract ISagaMessageSink<TSaga, TMessage> CreateSink(ISagaRepository<TSaga> repository,
		                                                                ISagaPolicy<TSaga, TMessage> policy);
	}
}