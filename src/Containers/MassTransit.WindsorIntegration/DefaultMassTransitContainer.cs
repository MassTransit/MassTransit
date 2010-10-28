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
    using Castle.Facilities.FactorySupport;
    using Castle.Windsor;
	using Castle.Windsor.Configuration;

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
			WindsorContainerConfigurator.RegisterInMemorySubscriptionRepository(this);
		}

		public void RegisterInMemorySagaRepository()
		{
			WindsorContainerConfigurator.RegisterInMemorySagaRepository(this);
		}

		private void Initialize()
		{
			_objectBuilder = WindsorContainerConfigurator.InitializeContainer(this);
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