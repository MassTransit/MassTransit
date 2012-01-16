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

	public interface InstanceSubscriptionConnector :
		InstanceConnector
	{
		Type MessageType { get; }
	}

	public class InstanceSubscriptionConnector<TConsumer, TMessage> :
		InstanceSubscriptionConnector
		where TConsumer : class, Consumes<TMessage>.All
		where TMessage : class
	{
		public Type MessageType
		{
			get { return typeof (TMessage); }
		}

		public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, object instance)
		{
			var consumer = instance as TConsumer;
			if (consumer == null)
				throw new NullReferenceException("The consumer instance cannot be null.");

			var sink = new InstanceMessageSink<TMessage>(MultipleHandlerSelector.ForHandler(
				HandlerSelector.ForHandler<TMessage>(consumer.Consume)));

			return configurator.Pipeline.ConnectToRouter(sink, () => configurator.SubscribedTo<TMessage>());
		}
	}
}