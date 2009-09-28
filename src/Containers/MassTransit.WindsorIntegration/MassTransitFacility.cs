// Copyright 2007-2008 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//   http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.WindsorIntegration
{
	using System;
	using System.ComponentModel;
	using Castle.Core.Configuration;
	using Castle.Facilities.Startable;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.Registration;
	using Configuration;
	using Exceptions;
	using Magnum.ObjectExtensions;
	using Services.HealthMonitoring.Configuration;
	using Services.Subscriptions.Configuration;
	using Component=Castle.MicroKernel.Registration.Component;

	/// <summary>
	/// Facility to simplify the use of MT
	/// </summary>
	public class MassTransitFacility :
		AbstractFacility
	{
		protected override void Init()
		{
			ConfigureEndpointFactory();

			ConfigureServiceBus();

			//Kernel.Resolver.AddSubResolver(new ServiceLocatorResolver());
			SetupAutoRegister();
			//AddStartableFacility();
		}

		private void ConfigureEndpointFactory()
		{
			var transportConfiguration = FacilityConfig.Children["transports"];
			if (transportConfiguration == null)
				throw new ConventionException("At least one transport must be defined in the facility configuration.");

			RegisterEndpointFactory(x =>
				{
					foreach (IConfiguration transport in transportConfiguration.Children)
					{
						Type transportType = Type.GetType(transport.Value, true, true);

						x.RegisterTransport(transportType);
					}
				});
		}

		private void ConfigureServiceBus()
		{
			foreach (IConfiguration child in FacilityConfig.Children)
			{
				if (child.Name.Equals("bus"))
				{
					var busConfig = child;

					string id = busConfig.Attributes["id"];
					string endpointUri = busConfig.Attributes["endpoint"];

					RegisterServiceBus(id, endpointUri, x =>
						{
							ConfigureThreadingModel(busConfig, x);

							ConfigureSubscriptionClient(busConfig, x);

							ConfigureManagementClient(busConfig, x);

							ConfigureControlBus(busConfig, x);
						});
				}
			}
		}

		private void RegisterEndpointFactory(Action<IEndpointFactoryConfigurator> configAction)
		{
			var endpointFactory = EndpointFactoryConfigurator.New(x =>
				{
					x.SetObjectBuilder(Kernel.Resolve<IObjectBuilder>());
					configAction(x);
				});


			Kernel.AddComponentInstance("endpointFactory", typeof (IEndpointFactory), endpointFactory);
		}

		private IServiceBus RegisterServiceBus(string id, string endpointUri, Action<IServiceBusConfigurator> configAction)
		{
			IServiceBus bus = ServiceBusConfigurator.New(x =>
				{
					x.ReceiveFrom(endpointUri);

					configAction(x);
				});

			Kernel.AddComponentInstance(id, typeof (IServiceBus), bus);

			return bus;
		}

		private IControlBus RegisterControlBus(string id, string endpointUri, Action<IServiceBusConfigurator> configAction)
		{
			IControlBus bus = ControlBusConfigurator.New(x =>
				{
					x.ReceiveFrom(endpointUri);
					x.SetConcurrentReceiverLimit(1);

					configAction(x);
				});

			Kernel.AddComponentInstance(id, typeof (IControlBus), bus);

			return bus;
		}

		public void AddStartableFacility()
		{
			foreach (IFacility facility in Kernel.GetFacilities())
			{
				if (facility.GetType().Equals(typeof (StartableFacility)))
				{
					return;
				}
			}
		}

		private void SetupAutoRegister()
		{
			if (Convert.ToBoolean(FacilityConfig.Attributes["auto-register"]))
			{
				Kernel.ComponentRegistered += OnComponentRegistered;
			}
		}

		private void OnComponentRegistered(string key, IHandler handler)
		{
			//TODO: Could throw
			var bus = this.Kernel.Resolve<IServiceBus>();
			Type consumerType = typeof (IConsumer);
			if (consumerType.IsAssignableFrom(handler.ComponentModel.Implementation))
			{
				// TODO this is broken and needs to use something better to invoke the generic method on IServiceBus

				bus.Subscribe(handler.ComponentModel.Implementation);
			}
		}

		private static void ConfigureThreadingModel(IConfiguration busConfig, IServiceBusConfigurator configurator)
		{
			WithConfig(busConfig, "dispatcher", config =>
				{
					GetConfigurationValue<int>(config, "readThreads", configurator.SetConcurrentConsumerLimit);
					GetConfigurationValue<int>(config, "maxThreads", configurator.SetConcurrentReceiverLimit);
				});
		}

		private static void ConfigureSubscriptionClient(IConfiguration busConfig, IServiceBusConfigurator configurator)
		{
			WithConfig(busConfig, "subscriptionService", config =>
				{
					string subscriptionServiceEndpointUri = config.Attributes["endpoint"];
					if (string.IsNullOrEmpty(subscriptionServiceEndpointUri))
						throw new ConfigurationException("The endponit for the subscriptionService cannot be null or empty");

					configurator.ConfigureService<SubscriptionClientConfigurator>(x =>
						{
							x.SetSubscriptionServiceEndpoint(subscriptionServiceEndpointUri);
						});
				});
		}

		private static void ConfigureManagementClient(IConfiguration busConfig, IServiceBusConfigurator configurator)
		{
			WithConfig(busConfig, "managementService", config =>
				{
					string heartbeatInterval = config.Attributes["heartbeatInterval"];

					int interval = heartbeatInterval.IsNullOrEmpty() ? 3 : int.Parse(heartbeatInterval);

					configurator.ConfigureService<HealthClientConfigurator>(x =>
						{
							x.SetHeartbeatInterval(interval);
						});
				});
		}

		private void ConfigureControlBus(IConfiguration busConfig, IServiceBusConfigurator configurator)
		{
			WithConfig(busConfig, "controlBus", config =>
			{
				string id = config.Attributes["id"];
				string endpointUri = config.Attributes["endpoint"];

				IControlBus bus = RegisterControlBus(id, endpointUri, x =>
				{
					ConfigureThreadingModel(busConfig, x);
				});

				configurator.UseControlBus(bus);
			});
		}

		private static void WithConfig(IConfiguration configuration, string key, Action<IConfiguration> action)
		{
			var config = configuration.Children[key];
			if (config != null)
				action(config);
		}


		private static ComponentRegistration<T> StartableComponent<T>()
		{
			return Component.For<T>()
				.AddAttributeDescriptor("startable", "true")
				.AddAttributeDescriptor("startMethod", "Start")
				.AddAttributeDescriptor("stopMethod", "Stop")
				.LifeStyle.Transient;
		}

		private static void GetConfigurationValue<T>(IConfiguration config, string attributeName, Action<T> applyValue)
		{
			string value = config.Attributes[attributeName];
			if (string.IsNullOrEmpty(value))
				return;

			TypeConverter tc = TypeDescriptor.GetConverter(typeof (T));

			T newValue = (T) tc.ConvertFromInvariantString(value);

			applyValue(newValue);
		}
	}
}