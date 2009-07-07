// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Configuration
{
	using System;
	using System.Collections.Generic;
	using Exceptions;
	using Internal;
	using log4net;
	using Pipeline.Configuration;

    public class ServiceBusConfigurator :
		ServiceBusConfiguratorDefaults,
		IServiceBusConfigurator
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (ServiceBusConfigurator));

		private static readonly ServiceBusConfiguratorDefaults _defaults = new ServiceBusConfiguratorDefaults();
		private readonly List<Action<IServiceBus, IObjectBuilder, Action<Type, IBusService>>> _services;
		private Uri _receiveFromUri;
	    private Action _beforeConsume;
	    private Action _afterConsume;

	    protected ServiceBusConfigurator()
		{
			_services = new List<Action<IServiceBus, IObjectBuilder, Action<Type, IBusService>>>();

			_defaults.ApplyTo(this);
		}

		protected IControlBus ControlBus { get; set; }

		public void ReceiveFrom(string uriString)
		{
			try
			{
				_receiveFromUri = new Uri(uriString);
			}
			catch (UriFormatException ex)
			{
				throw new ConfigurationException("The Uri for the receive endpoint is invalid: " + uriString, ex);
			}
		}

		public void ReceiveFrom(Uri uri)
		{
			_receiveFromUri = uri;
		}

		public void ConfigureService<TServiceConfigurator>(Action<TServiceConfigurator> configure)
			where TServiceConfigurator : IServiceConfigurator, new()
		{
			_services.Add((bus, builder, add) =>
				{
					TServiceConfigurator configurator = new TServiceConfigurator();

					configure(configurator);

					var service = configurator.Create(bus, builder);

					add(configurator.ServiceType, service);
				});
		}

		public void AddService<TService>()
			where TService : IBusService
		{
			_services.Add((bus, builder, add) =>
				{
					DefaultBusServiceConfigurator<TService> configurator = new DefaultBusServiceConfigurator<TService>();

					var service = configurator.Create(bus, builder);

					add(configurator.ServiceType, service);
				});
		}

		public void UseControlBus(IControlBus bus)
		{
			ControlBus = bus;
		}

	    public void BeforeConsumingMessage(Action beforeConsume)
	    {
	        if (_beforeConsume == null)
	            _beforeConsume = beforeConsume;
	        else
	            _beforeConsume += beforeConsume;
	    }

	    public void AfterConsumingMessage(Action afterConsume)
	    {
            if (_afterConsume == null)
                _afterConsume = afterConsume;
            else
                _afterConsume += afterConsume;
	    }

	    internal IControlBus Create()
		{
			ServiceBus bus = CreateServiceBus();

			ConfigurePoisonEndpoint(bus);

			ConfigureThreadLimits(bus);

			if (AutoSubscribe)
			{
				// get all the types and subscribe them to the bus
			}

	        ConfigureMessageInterceptors(bus);

			ConfigureControlBus(bus);

			ConfigureBusServices(bus);

			if (AutoStart)
			{
				bus.Start();
			}

			return bus;
		}

	    private void ConfigureMessageInterceptors(IServiceBus bus)
	    {
	        if(_beforeConsume != null || _afterConsume != null)
	        {
	            MessageInterceptorConfigurator.For(bus.InboundPipeline).Create(_beforeConsume, _afterConsume);
	        }
	    }

	    private void ConfigureControlBus(ServiceBus bus)
		{
			if (ControlBus == null) 
				return;

			if(_log.IsDebugEnabled)
				_log.DebugFormat("Associating control bus ({0}) with service bus ({1})", ControlBus.Endpoint.Uri, bus.Endpoint.Uri);

			bus.ControlBus = ControlBus;
		}

		private void ConfigurePoisonEndpoint(ServiceBus bus)
		{
			if (ErrorUri != null)
			{
				bus.PoisonEndpoint = bus.EndpointFactory.GetEndpoint(ErrorUri);
			}
		}

		private ServiceBus CreateServiceBus()
		{
			var endpointFactory = ObjectBuilder.GetInstance<IEndpointFactory>();

			var endpoint = endpointFactory.GetEndpoint(_receiveFromUri);

			return new ServiceBus(endpoint, ObjectBuilder, endpointFactory);
		}

		private void ConfigureThreadLimits(ServiceBus bus)
		{
			if (ConcurrentConsumerLimit > 0)
				bus.MaximumConsumerThreads = ConcurrentConsumerLimit;

			if (ConcurrentReceiverLimit > 0)
				bus.ConcurrentReceiveThreads = ConcurrentReceiverLimit;

			bus.ReceiveTimeout = ReceiveTimeout;
		}

		private void ConfigureBusServices(ServiceBus bus)
		{
			foreach (var serviceConfigurator in _services)
			{
				serviceConfigurator(bus, ObjectBuilder, bus.AddService);
			}
		}

		public static IServiceBus New(Action<IServiceBusConfigurator> action)
		{
			var configurator = new ServiceBusConfigurator();

			action(configurator);

			return configurator.Create();
		}

		public static void Defaults(Action<IServiceBusConfiguratorDefaults> action)
		{
			action(_defaults);
		}
	}

	public class ControlBusConfigurator :
		ServiceBusConfigurator
	{
		public new static IControlBus New(Action<IServiceBusConfigurator> action)
		{
			var configurator = new ControlBusConfigurator();
			configurator.SetConcurrentConsumerLimit(1);

			action(configurator);

			return configurator.Create();
		}
	}
}