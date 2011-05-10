namespace MassTransit
{
	using System;
	using BusConfigurators;
	using EndpointConfigurators;
	using Transports.RabbitMq;

	public static class RabbitMqServiceBusExtensions
	{
		/// <summary>
		/// Returns the endpoint for the specified message type using the default
		/// exchange/queue convention for naming
		/// </summary>
		/// <typeparam name="TMessage">The message type to convert to a URI</typeparam>
		/// <param name="bus">The bus instance used to resolve the endpoint</param>
		/// <returns>The IEndpoint instance, resolved from the service bus</returns>
		public static IEndpoint GetEndpoint<TMessage>(this IServiceBus bus)
			where TMessage : class
		{
			return GetEndpoint(bus, typeof (TMessage));
		}

		/// <summary>
		/// Returns the endpoint for the specified message type using the default
		/// exchange/queue convention for naming
		/// </summary>
		/// <param name="bus">The bus instance used to resolve the endpoint</param>
		/// <param name="messageType">The message type to convert to a URI</param>
		/// <returns>The IEndpoint instance, resolved from the service bus</returns>
		public static IEndpoint GetEndpoint(this IServiceBus bus, Type messageType)
		{
			return null;
		}

		public static ServiceBusConfigurator UseRabbitMq(this ServiceBusConfigurator configurator)
		{
			EndpointFactoryConfigurator endpointFactoryConfigurator = configurator;

			endpointFactoryConfigurator.UseRabbitMq();

			return configurator;
		}

		public static T UseRabbitMq<T>(this T configurator)
			where T : EndpointFactoryConfigurator
		{
			configurator.AddTransportFactory<RabbitMqTransportFactory>();
			configurator.UseJsonSerializer();

			return configurator;
		}
	}
}