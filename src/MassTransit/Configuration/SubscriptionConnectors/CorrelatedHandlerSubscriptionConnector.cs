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
namespace MassTransit.SubscriptionConnectors
{
	using System;
	using Pipeline;
	using Pipeline.Configuration;
	using Pipeline.Sinks;

	public class CorrelatedHandlerSubscriptionConnector<TMessage, TKey> :
		HandlerSubscriptionConnector<TMessage>
		where TMessage : class, CorrelatedBy<TKey>
	{
		public Type MessageType
		{
			get { return typeof (TMessage); }
		}

		public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, TKey correlationId, HandlerSelector<TMessage> handler)
		{
			if (handler == null)
				throw new NullReferenceException("The handler cannot be null.");

			var correlatedConfigurator = new InboundCorrelatedMessageRouterConfigurator(configurator.Pipeline);

			CorrelatedMessageRouter<IConsumeContext<TMessage>, TMessage, TKey> router =
				correlatedConfigurator.FindOrCreate<TMessage, TKey>();

			var sink = new InstanceMessageSink<TMessage>(MultipleHandlerSelector.ForHandler(handler));

			UnsubscribeAction result = router.Connect(correlationId, sink);

			UnsubscribeAction remove = configurator.SubscribedTo<TMessage, TKey>(correlationId);

			return () => result() && (router.SinkCount(correlationId) == 0) && remove();
		}
	}
}