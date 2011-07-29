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
	using Distributor.SubscriptionConnectors;
	using Exceptions;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using Pipeline;
	using Saga;
	using Util;

	public interface InstanceConnector
	{
		UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, object instance);
	}

	public class InstanceConnector<T> :
		InstanceConnector
		where T : class
	{
		readonly IEnumerable<InstanceSubscriptionConnector> _connectors;

		public InstanceConnector()
		{
			Type[] interfaces = typeof (T).GetInterfaces();

			if (interfaces.Contains(typeof (ISaga)))
				throw new ConfigurationException("A saga cannot be registered as a consumer");

			if (interfaces.Implements(typeof (InitiatedBy<>))
			    || interfaces.Implements(typeof (Orchestrates<>))
			    || interfaces.Implements(typeof (Observes<,>)))
				throw new ConfigurationException("InitiatedBy, Orchestrates, and Observes can only be used with sagas");

			_connectors = Distributors()
				.Concat(Workers())
				.Concat(ConsumesCorrelated())
				.Concat(ConsumesContext())
				.Concat(ConsumesSelected())
				.Concat(ConsumesAll())
				.Distinct((x, y) => x.MessageType == y.MessageType)
				.ToList();
		}


		public IEnumerable<InstanceSubscriptionConnector> Connectors
		{
			get { return _connectors; }
		}

		public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, object instance)
		{
			return _connectors.Select(x => x.Connect(configurator, instance))
				.Aggregate<UnsubscribeAction, UnsubscribeAction>(() => true, (seed, x) => () => seed() && x());
		}

		IEnumerable<InstanceSubscriptionConnector> ConsumesContext()
		{
			return typeof(T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof(Consumes<>.All))
				.Select(x => new { InterfaceType = x, MessageType = x.GetGenericArguments()[0] })
				.Where(x => x.MessageType.IsGenericType)
				.Where(x => x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>))
				.Select(x => new { x.InterfaceType, MessageType = x.MessageType.GetGenericArguments()[0] })
				.Where(x => x.MessageType.IsValueType == false)
				.Select(x =>
						FastActivator.Create(typeof(InstanceContextSubscriptionConnector<,>),
							new[] { typeof(T), x.MessageType }))
				.Cast<InstanceSubscriptionConnector>();
		}

		static IEnumerable<InstanceSubscriptionConnector> ConsumesAll()
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (Consumes<>.All))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsValueType == false)
				.Where(x => !(x.MessageType.IsGenericType && x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>)))
				.Select(x => FastActivator.Create(typeof (InstanceSubscriptionConnector<,>), new[] {typeof (T), x.MessageType}))
				.Cast<InstanceSubscriptionConnector>();
		}

		static IEnumerable<InstanceSubscriptionConnector> ConsumesSelected()
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (Consumes<>.Selected))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsValueType == false)
				.Where(x => !(x.MessageType.IsGenericType && x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>)))
				.Select(
					x => FastActivator.Create(typeof (SelectedInstanceSubscriptionConnector<,>), new[] {typeof (T), x.MessageType}))
				.Cast<InstanceSubscriptionConnector>();
		}

		static IEnumerable<InstanceSubscriptionConnector> ConsumesCorrelated()
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (Consumes<>.For<>))
				.Select(x => new
					{
						InterfaceType = x,
						MessageType = x.GetGenericArguments()[0],
						CorrelationType = x.GetGenericArguments()[1]
					})
				.Where(x => x.MessageType.IsValueType == false)
				.Where(x => !(x.MessageType.IsGenericType && x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>)))
	.Select(
					x =>
					typeof (CorrelatedInstanceSubscriptionConnector<,,>).MakeGenericType(typeof (T), x.MessageType, x.CorrelationType))
				.Select(x => FastActivator.Create(x))
				.Cast<InstanceSubscriptionConnector>();
		}

		static IEnumerable<InstanceSubscriptionConnector> Distributors()
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (IDistributor<>))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsValueType == false)
				.Where(x => !(x.MessageType.IsGenericType && x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>)))
				.Select(x => FastActivator.Create(typeof(DistributorSubscriptionConnector<>), new[] { x.MessageType }))
				.Cast<InstanceSubscriptionConnector>();
		}

		static IEnumerable<InstanceSubscriptionConnector> Workers()
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (IWorker<>))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsValueType == false)
				.Where(x => !(x.MessageType.IsGenericType && x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>)))
				.Select(x => FastActivator.Create(typeof (WorkerSubscriptionConnector<>), new[] {x.MessageType}))
				.Cast<InstanceSubscriptionConnector>();
		}
	}
}