/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.Host2
{
	using System;
    using Castle.Facilities.FactorySupport;
    using Castle.Facilities.Startable;
	using Castle.Windsor;
    using ServiceBus;
    using WindsorIntegration;

    public abstract class HostedEnvironment
    {
        private IWindsorContainer _container;


        protected HostedEnvironment(string xmlFile) 
        {
            _container = new WindsorContainer(xmlFile);
            _container.AddFacility("masstransit", new MassTransitFacility());
            _container.AddFacility("factory", new FactorySupportFacility());
            _container.AddFacility("startable", new StartableFacility());
        }
        protected HostedEnvironment()
        {
            _container = new WindsorContainer();
        }


        public IWindsorContainer Container
        {
            get { return _container; }
        }

        public virtual void Start()
        {
            //move this into interceptor?
            foreach (IHostedService hs in _container.ResolveAll<IHostedService>())
            {
                hs.Start();
            }
        }
        public virtual void Stop()
        {
            //move this into interceptor?
            foreach (IHostedService hs in _container.ResolveAll<IHostedService>())
            {
                hs.Stop();
            }

        	foreach (IServiceBus bus in _container.ResolveAll<IServiceBus>())
        	{
        		bus.Dispose();
        		_container.Release(bus);
        	}
        }

        public abstract string ServiceName {get;}
        public abstract string DispalyName {get;}
        public abstract string Description {get;}

    	public event Action<HostedEnvironment> Completed;

    	internal delegate void Handler();
    }
}
