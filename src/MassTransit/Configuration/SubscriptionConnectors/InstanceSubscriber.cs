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

	public class InstanceSubscriber<T> :
		InstanceConnector
		where T : class
	{
		readonly IEnumerable<InstanceSubscriptionConnector> _connectors;

		public InstanceSubscriber()
		{
			Type[] interfaces = typeof (T).GetInterfaces();

			if (interfaces.Contains(typeof (ISaga)))
				throw new ConfigurationException("A saga cannot be registered as a consumer");

			if (interfaces.Implements(typeof (InitiatedBy<>))
			    || interfaces.Implements(typeof (Orchestrates<>))
			    || interfaces.Implements(typeof (Observes<,>)))
				throw new ConfigurationException("InitiatedBy, Orchestrates, and Observes can only be used with sagas");

			_connectors = ConsumesCorrelated()
				.Union(ConsumesSelected())
				.Union(ConsumesAll())
				.Distinct((x, y) => x.MessageType == y.MessageType)
				.ToList();
		}


		public UnsubscribeAction Connect(IPipelineConfigurator configurator, object instance)
		{
			return _connectors.Select(x => x.Connect(configurator, instance))
				.Aggregate<UnsubscribeAction,UnsubscribeAction>(() => true, (seed, x) => () => seed() && x());
		}

		public IEnumerable<InstanceSubscriptionConnector> Connectors
		{
			get { return _connectors; }
		}

		static IEnumerable<InstanceSubscriptionConnector> ConsumesAll()
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (Consumes<>.All))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsValueType == false)
				.Select(x => typeof (InstanceSubscriptionConnector<,>).MakeGenericType(typeof (T), x.MessageType))
				.Select(x => FastActivator.Create(x))
				.Cast<InstanceSubscriptionConnector>();
		}

		static IEnumerable<InstanceSubscriptionConnector> ConsumesSelected()
		{
			return typeof (T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof (Consumes<>.Selected))
				.Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
				.Where(x => x.MessageType.IsValueType == false)
				.Select(x => typeof (SelectedInstanceSubscriptionConnector<,>).MakeGenericType(typeof (T), x.MessageType))
				.Select(x => FastActivator.Create(x))
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
				.Select(
					x =>
					typeof (CorrelatedInstanceSubscriptionConnector<,,>).MakeGenericType(typeof (T), x.MessageType, x.CorrelationType))
				.Select(x => FastActivator.Create(x))
				.Cast<InstanceSubscriptionConnector>();
		}
	}
}