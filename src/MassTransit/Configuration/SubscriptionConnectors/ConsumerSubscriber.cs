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
	using Exceptions;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using Pipeline;
	using Pipeline.Sinks;
	using Saga;
	using Util;

	public class ConsumerSubscriber<T>
		where T : class
	{
		readonly IConsumerFactory<T> _consumerFactory;
		readonly IEnumerable<ConsumerSubscriptionConnector> _connectors;

		public ConsumerSubscriber(IConsumerFactory<T> consumerFactory)
		{
			_consumerFactory = consumerFactory;
			Type[] interfaces = typeof (T).GetInterfaces();

			if (interfaces.Contains(typeof (ISaga)))
				throw new ConfigurationException("A saga cannot be registered as a consumer");

			if (interfaces.Implements(typeof (InitiatedBy<>))
			    || interfaces.Implements(typeof (Orchestrates<>))
			    || interfaces.Implements(typeof (Observes<,>)))
				throw new ConfigurationException("InitiatedBy, Orchestrates, and Observes can only be used with sagas");

			_connectors = ConsumesSelected(consumerFactory)
				.Union(ConsumesAll(consumerFactory))
				.Distinct((x, y) => x.MessageType == y.MessageType)
				.ToList();
		}

		public UnsubscribeAction Connect(IPipelineConfigurator configurator)
		{
			return _connectors.Select(x => x.Connect(configurator))
				.Aggregate<UnsubscribeAction,UnsubscribeAction>(() => true, (seed, x) => () => seed() && x());
		}

		public IEnumerable<ConsumerSubscriptionConnector> Connectors
		{
			get { return _connectors; }
		}

		static IEnumerable<ConsumerSubscriptionConnector> ConsumesAll(IConsumerFactory<T> consumerFactory)
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (Consumes<>.All))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsValueType == false)
				.Select(x => typeof (ConsumerSubscriptionConnector<,>).MakeGenericType(typeof (T), x.MessageType))
				.Select(x => FastActivator.Create(x, consumerFactory))
				.Cast<ConsumerSubscriptionConnector>();
		}

		static IEnumerable<ConsumerSubscriptionConnector> ConsumesSelected(IConsumerFactory<T> consumerFactory)
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (Consumes<>.Selected))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsValueType == false)
				.Select(x => typeof (SelectedConsumerSubscriptionConnector<,>).MakeGenericType(typeof (T), x.MessageType))
				.Select(x => FastActivator.Create(x, consumerFactory))
				.Cast<ConsumerSubscriptionConnector>();
		}
	}
}