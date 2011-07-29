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
	using System.Collections.Generic;
	using System.Linq;
	using Distributor;
	using Exceptions;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using Pipeline;
	using Saga;
	using Util;

	public interface ConsumerConnector
	{
		UnsubscribeAction Connect(IInboundPipelineConfigurator configurator);
	}

	public class ConsumerConnector<T> :
		ConsumerConnector
		where T : class
	{
		readonly object[] _args;
		readonly IEnumerable<ConsumerSubscriptionConnector> _connectors;

		public ConsumerConnector(IConsumerFactory<T> consumerFactory)
		{
			_args = new object[] {consumerFactory};

			Type[] interfaces = typeof (T).GetInterfaces();

			if (interfaces.Contains(typeof (ISaga)))
				throw new ConfigurationException("A saga cannot be registered as a consumer");

			if (interfaces.Implements(typeof (InitiatedBy<>))
			    || interfaces.Implements(typeof (Orchestrates<>))
			    || interfaces.Implements(typeof (Observes<,>)))
				throw new ConfigurationException("InitiatedBy, Orchestrates, and Observes can only be used with sagas");

			if (interfaces.Implements(typeof (IDistributor<>))
			    || interfaces.Implements(typeof (IWorker<>))
			    || interfaces.Implements(typeof (ISagaWorker<>)))
				throw new ConfigurationException("Distributor classes can only be subscribed as instances");

			_connectors = ConsumesContext()
				.Concat(ConsumesSelected())
				.Concat(ConsumesAll())
				.Distinct((x, y) => x.MessageType == y.MessageType)
				.ToList();
		}

		public IEnumerable<ConsumerSubscriptionConnector> Connectors
		{
			get { return _connectors; }
		}

		public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator)
		{
			return _connectors.Select(x => x.Connect(configurator))
				.Aggregate<UnsubscribeAction, UnsubscribeAction>(() => true, (seed, x) => () => seed() && x());
		}

		IEnumerable<ConsumerSubscriptionConnector> ConsumesContext()
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (Consumes<>.All))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsGenericType)
				.Where(x => x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>))
				.Select(x => new {x.InterfaceType, MessageType = x.MessageType.GetGenericArguments()[0] })
				.Where(x => x.MessageType.IsValueType == false)
				.Select(x =>
				        FastActivator.Create(typeof (ConsumerContextSubscriptionConnector<,>),
				        	new[] {typeof (T), x.MessageType}, _args))
				.Cast<ConsumerSubscriptionConnector>();
		}

		IEnumerable<ConsumerSubscriptionConnector> ConsumesAll()
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (Consumes<>.All))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsValueType == false)
				.Where(x => !(x.MessageType.IsGenericType && x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>)))
				.Select(x => 
					FastActivator.Create(typeof (ConsumerSubscriptionConnector<,>), 
					new[] {typeof (T), x.MessageType}, _args))
				.Cast<ConsumerSubscriptionConnector>();
		}

		IEnumerable<ConsumerSubscriptionConnector> ConsumesSelected()
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (Consumes<>.Selected))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsValueType == false)
				.Where(x => !(x.MessageType.IsGenericType && x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>)))
				.Select(x =>
					FastActivator.Create(typeof (SelectedConsumerSubscriptionConnector<,>), 
					new[] {typeof (T), x.MessageType}, _args))
				.Cast<ConsumerSubscriptionConnector>();
		}
	}
}