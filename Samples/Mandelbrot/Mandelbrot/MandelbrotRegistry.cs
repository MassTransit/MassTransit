namespace Mandelbrot
{
	using System;
	using Core;
	using MassTransit;
	using MassTransit.Configuration;
	using MassTransit.Grid;
	using MassTransit.Internal;
	using MassTransit.Serialization;
	using MassTransit.StructureMapIntegration;
	using MassTransit.Subscriptions;
	using MassTransit.Transports;
	using StructureMap;
	using StructureMap.Attributes;
	using StructureMap.Configuration.DSL;

	public class MandelbrotRegistry :
		Registry
	{
		private readonly Uri _endpointToListenOn;

		public MandelbrotRegistry(string uriString)
		{
			_endpointToListenOn = new Uri(uriString);

			RegisterBusDependencies();

			RegisterEndpointFactory();

			RegisterServiceBus();

			RegisterMandelbrotTypes();
		}

		private void RegisterMandelbrotTypes()
		{
			ForRequestedType<SubTaskWorker<GenerateMandelbrotWorker, GenerateRow, RowGenerated>>()
				.TheDefaultIsConcreteType<SubTaskWorker<GenerateMandelbrotWorker, GenerateRow, RowGenerated>>();
		}

		private void RegisterBusDependencies()
		{
			ForRequestedType<IObjectBuilder>()
				.TheDefaultIsConcreteType<StructureMapObjectBuilder>()
				.CacheBy(InstanceScope.Singleton);
			ForRequestedType<ISubscriptionCache>()
				.TheDefaultIsConcreteType<LocalSubscriptionCache>()
				.CacheBy(InstanceScope.Singleton);
			ForRequestedType<ITypeInfoCache>()
				.TheDefaultIsConcreteType<TypeInfoCache>()
				.CacheBy(InstanceScope.Singleton);
		}

		private void RegisterEndpointFactory()
		{
			ForRequestedType<IEndpointFactory>()
				.CacheBy(InstanceScope.Singleton)
				.TheDefault.Is.ConstructedBy(context =>
					{
						return EndpointFactoryConfigurator.New(x =>
							{
								x.SetObjectBuilder(ObjectFactory.GetInstance<IObjectBuilder>());
								x.RegisterTransport<LoopbackEndpoint>();
								x.SetDefaultSerializer<BinaryMessageSerializer>();
							});
					});
		}

		private void RegisterServiceBus()
		{
			ForRequestedType<IServiceBus>()
				.CacheBy(InstanceScope.Singleton)
				.TheDefault.Is.ConstructedBy(context =>
					{
						return ServiceBusConfigurator.New(x =>
							{
								x.SetObjectBuilder(ObjectFactory.GetInstance<IObjectBuilder>());
								x.ReceiveFrom(_endpointToListenOn);
								x.SetConcurrentConsumerLimit(Environment.ProcessorCount * 2);
								x.SetConcurrentReceiverLimit(Environment.ProcessorCount);
							});
					});
		}
	}
}