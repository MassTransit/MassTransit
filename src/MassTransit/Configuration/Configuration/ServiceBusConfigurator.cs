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
	using BusConfigurators;
	using BusServiceConfigurators;
	using Configurators;
	using Exceptions;
	using Internal;
	using log4net;
	using Pipeline.Configuration;

	[Obsolete("Replaced by ServiceBusFactory")]
    public class ServiceBusConfigurator : 
		IServiceBusConfiguratorDefaults
    {
		private static readonly ILog _log = LogManager.GetLogger(typeof (ServiceBusConfigurator));

		private readonly List<Action<IServiceBus, IObjectBuilder, Action<Type, IBusService>>> _services;

        //CHANGED TO SUPPORT THE MOVE TO THE NEXT CONFIG MODEL
	    ServiceBusConfigurator()
		{
			_services = new List<Action<IServiceBus, IObjectBuilder, Action<Type, IBusService>>>();
		}

		protected IControlBus ControlBus { get; set; }

		public void ConfigureService<TServiceConfigurator>(Action<TServiceConfigurator> configure) where TServiceConfigurator : BusServiceConfigurator, new()
		{
			_services.Add((bus, builder, add) =>
				{
					TServiceConfigurator configurator = new TServiceConfigurator();

					configure(configurator);

					var service = configurator.Create(bus, builder);

					add(configurator.ServiceType, service);
				});
		}

		public void AddService<TService>(Func<TService> getService)
			where TService : IBusService
		{
			_services.Add((bus, builder, add) =>
				{
					var service = getService();

					add(typeof (TService), service);
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

		private void ConfigureBusServices(ServiceBus bus)
		{
			foreach (var serviceConfigurator in _services)
			{
				serviceConfigurator(bus, ObjectBuilder, bus.AddService);
			}
		}

		public static IServiceBus New(Action<ServiceBusConfigurator> action)
		{
			var configurator = new ServiceBusConfiguratorImpl(TODO);

			action(configurator);

			return configurator.CreateServiceBus();
		}

	}

}