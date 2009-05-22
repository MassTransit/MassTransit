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
namespace MassTransit.WindsorIntegration
{
	using Castle.Facilities.Startable;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using Castle.Windsor.Configuration;
	using Configuration;
	using Microsoft.Practices.ServiceLocation;
	using Saga;
	using Saga.Pipeline;
	using Serialization;
	using Services.Subscriptions.Client;
	using Services.Subscriptions.Server;

	public class DefaultMassTransitContainer :
		WindsorContainer
	{
	    private IObjectBuilder _objectBuilder;

		public DefaultMassTransitContainer()
		{
			Initialize();
		}

		public DefaultMassTransitContainer(string xmlFile)
			: base(xmlFile)
		{
			Initialize();

			AddMassTransitFacility();
		}

		public DefaultMassTransitContainer(IConfigurationInterpreter configurationInterpreter)
			: base(configurationInterpreter)
		{
			Initialize();

			AddMassTransitFacility();
		}

		public void RegisterInMemorySubscriptionRepository()
		{
			Kernel.Register(
				Component.For<ISubscriptionRepository>()
					.ImplementedBy<InMemorySubscriptionRepository>()
					.LifeStyle.Singleton
				);
		}

		public void RegisterInMemorySagaRepository()
		{
			Kernel.Register(
				Component.For(typeof(ISagaRepository<>))
					.ImplementedBy(typeof(InMemorySagaRepository<>))
					.LifeStyle.Singleton
				);
		}


		private void Initialize()
		{
			var wob = new WindsorObjectBuilder(Kernel);
		    _objectBuilder = wob;
			ServiceLocator.SetLocatorProvider(() => wob);

			Kernel.AddComponentInstance("kernel", typeof (IKernel), Kernel);
			Kernel.AddComponentInstance("objectBuilder", typeof (IObjectBuilder), wob);

			Register(
				Component.For<IObjectBuilder>()
					.ImplementedBy<WindsorObjectBuilder>()
					.LifeStyle.Singleton,

				// The subscription client
				Component.For<SubscriptionClient>()
					.ImplementedBy<SubscriptionClient>()
					.LifeStyle.Transient,


				Component.For(typeof(InitiatingSagaPolicy<,>))
					.ImplementedBy(typeof(InitiatingSagaPolicy<,>))
					.LifeStyle.Transient,
				Component.For(typeof(ExistingSagaPolicy<,>))
					.ImplementedBy(typeof(ExistingSagaPolicy<,>))
					.LifeStyle.Transient,

				Component.For(typeof(ExistingOrIgnoreSagaPolicy<,>))
					.ImplementedBy(typeof(ExistingOrIgnoreSagaPolicy<,>))
					.LifeStyle.Transient,

				Component.For(typeof(CreateOrUseExistingSagaPolicy<,>))
					.ImplementedBy(typeof(CreateOrUseExistingSagaPolicy<,>))
					.LifeStyle.Transient,


				// Saga message sinks
				Component.For(typeof (CorrelatedSagaMessageSink<,>))
					.ImplementedBy(typeof(CorrelatedSagaMessageSink<,>))
					.LifeStyle.Transient,
				Component.For(typeof(CorrelatedSagaStateMachineMessageSink<,>))
					.ImplementedBy(typeof(CorrelatedSagaStateMachineMessageSink<,>))
					.LifeStyle.Transient,

				Component.For(typeof (PropertySagaMessageSink<,>))
					.ImplementedBy(typeof(PropertySagaMessageSink<,>))
					.LifeStyle.Transient,
				Component.For(typeof(PropertySagaStateMachineMessageSink<,>))
					.ImplementedBy(typeof(PropertySagaStateMachineMessageSink<,>))
					.LifeStyle.Transient,

				// Message Serializers
				Component.For<BinaryMessageSerializer>()
					.ImplementedBy<BinaryMessageSerializer>()
					.LifeStyle.Singleton,
				Component.For<JsonMessageSerializer>()
					.ImplementedBy<JsonMessageSerializer>()
					.LifeStyle.Singleton,
				Component.For<XmlMessageSerializer>()
					.ImplementedBy<XmlMessageSerializer>()
					.LifeStyle.Singleton,
				Component.For<CustomXmlMessageSerializer>()
					.ImplementedBy<CustomXmlMessageSerializer>()
					.LifeStyle.Singleton
				);

			ServiceBusConfigurator.Defaults(x =>
				{
					// setup all service bus instances to use the windsor builder by default
					x.SetObjectBuilder(wob);
				});
		}

	    public IObjectBuilder ObjectBuilder
	    {
	        get { return _objectBuilder; }
	    }

	    private void AddMassTransitFacility()
		{
			AddFacility("masstransit", new MassTransitFacility());
		}
	}
}