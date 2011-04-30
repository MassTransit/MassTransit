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
	using Saga;
	using Util;

	public interface SagaConnector
	{
		Type SagaType { get; }

		UnsubscribeAction Connect(IPipelineConfigurator configurator);
	}


	public class SagaConnector<T> :
		SagaConnector
		where T : class, ISaga
	{
		readonly IEnumerable<SagaSubscriptionConnector> _connectors;
		object[] _args;

		public SagaConnector(ISagaRepository<T> sagaRepository)
		{
			_args = new object[] {sagaRepository};

			Type[] interfaces = typeof (T).GetInterfaces();

			if (!interfaces.Contains(typeof (ISaga)))
				throw new ConfigurationException("The type specified is not a saga");

			_connectors = StateMachineEvents()
				.Union(Initiates())
				.Union(Orchestrates())
				.Union(Observes())
				.Distinct((x, y) => x.MessageType == y.MessageType)
				.ToList();
		}

		public IEnumerable<SagaSubscriptionConnector> Connectors
		{
			get { return _connectors; }
		}

		public Type SagaType
		{
			get { return typeof (T); }
		}

		public UnsubscribeAction Connect(IPipelineConfigurator configurator)
		{
			return _connectors.Select(x => x.Connect(configurator))
				.Aggregate<UnsubscribeAction, UnsubscribeAction>(() => true, (seed, x) => () => seed() && x());
		}

		IEnumerable<SagaSubscriptionConnector> Initiates()
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (InitiatedBy<>))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsValueType == false)
				.Select(x => FastActivator.Create(typeof (InitiatedBySagaSubscriptionConnector<,>),
					new[] {typeof (T), x.MessageType}, _args))
				.Cast<SagaSubscriptionConnector>();
		}

		IEnumerable<SagaSubscriptionConnector> Orchestrates()
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (Orchestrates<>))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsValueType == false)
				.Select(x => FastActivator.Create(typeof (OrchestratesSagaSubscriptionConnector<,>),
					new[] {typeof (T), x.MessageType}, _args))
				.Cast<SagaSubscriptionConnector>();
		}

		IEnumerable<SagaSubscriptionConnector> Observes()
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (Observes<,>))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsValueType == false)
				.Select(x => FastActivator.Create(typeof (ObservesSagaSubscriptionConnector<,>),
					new[] {typeof (T), x.MessageType}, _args))
				.Cast<SagaSubscriptionConnector>();
		}

		IEnumerable<SagaSubscriptionConnector> StateMachineEvents()
		{
			if (typeof (T).Implements(typeof (SagaStateMachine<>)))
			{
				var factory = (IEnumerable<SagaSubscriptionConnector>) FastActivator.Create(typeof (StateMachineSagaConnector<>),
					new[] {typeof (T)},
					_args);

				return factory;
			}

			return Enumerable.Empty<SagaSubscriptionConnector>();
		}
	}
}