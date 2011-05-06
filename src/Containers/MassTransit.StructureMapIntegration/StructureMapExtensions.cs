namespace MassTransit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Magnum.Extensions;
	using StructureMap;
	using StructureMapIntegration;
	using SubscriptionConfigurators;

	public static class StructureMapExtensions
	{
		public static void LoadFrom(this SubscriptionBusServiceConfigurator configurator, IContainer container)
		{
			IList<Type> concreteTypes = container
				.Model
				.PluginTypes
				.Where(x => x.PluginType.Implements<IConsumer>())
				.Select(i => i.PluginType)
				.ToList();

			if (concreteTypes.Count == 0)
				return;

			var consumerConfigurator = new StructureMapConsumerFactoryConfigurator(configurator, container);

			foreach (Type concreteType in concreteTypes)
			{
				consumerConfigurator.ConfigureConsumer(concreteType);
			}
		}

		public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
			this SubscriptionBusServiceConfigurator configurator, IContainer kernel)
			where TConsumer : class
		{
			var consumerFactory = new StructureMapConsumerFactory<TConsumer>(kernel);

			return configurator.Consumer(consumerFactory);
		}
	}
}