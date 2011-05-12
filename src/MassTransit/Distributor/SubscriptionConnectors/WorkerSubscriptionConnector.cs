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
namespace MassTransit.Distributor.SubscriptionConnectors
{
	using System;
	using Exceptions;
	using MassTransit.Pipeline;
	using MassTransit.SubscriptionConnectors;
	using Messages;
	using Pipeline;

	public class WorkerSubscriptionConnector<TMessage> :
		InstanceSubscriptionConnector
		where TMessage : class
	{
		static Action<Distributed<TMessage>> NullConsumer;

		public Type MessageType
		{
			get { return typeof (Distributed<TMessage>); }
		}

		public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, object instance)
		{
			var worker = instance as IWorker<TMessage>;
			if (worker == null)
				throw new ConfigurationException("The instance is not a distributor worker");

			var sink = new WorkerMessageSink<Distributed<TMessage>>(message =>
				{
					// rock it
					return worker.Accept(message) ? worker.Consume : NullConsumer;
				});

			return configurator.Pipeline.ConnectToRouter(sink, () => configurator.SubscribedTo<Distributed<TMessage>>());
		}
	}
}