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
namespace MassTransit.Host.LifeCycles
{
	using System;
	using Castle.Facilities.FactorySupport;
	using Castle.Facilities.Startable;
	using Castle.Windsor;
	using ServiceBus;
	using WindsorIntegration;

	public delegate void Work();

	public abstract class HostedLifeCycle :
		IApplicationLifeCycle
	{
		private readonly string _xmlFile;
		private IWindsorContainer _container;

		protected HostedLifeCycle(string xmlFile)
		{
			_xmlFile = xmlFile;
			_container = new WindsorContainer(_xmlFile);
			_container.AddFacility("masstransit", new MassTransitFacility());
			_container.AddFacility("factory", new FactorySupportFacility());
			_container.AddFacility("startable", new StartableFacility());
		}

		public void Initialize()
		{
			foreach (IHostedService hs in _container.ResolveAll<IHostedService>())
			{
				hs.Start();
			}
		}

		public abstract void Start();

		public abstract void Stop();

		public virtual string DefaultAction
		{
			get { return "console"; }
		}

		public void Dispose()
		{
			foreach (IHostedService hs in _container.ResolveAll<IHostedService>())
			{
				hs.Stop();
			}

			foreach (IServiceBus bus in _container.ResolveAll<IServiceBus>())
			{
				bus.Dispose();
				_container.Release(bus);
			}

			_container.Dispose();


			if (Completed != null)
			{
				Action<IApplicationLifeCycle> handler = Completed;
				handler(this);
			}
		}

		public IWindsorContainer Container
		{
			get { return _container; }
		}

		//TODO: WTF is this (and I wrote it!)
		public event Action<IApplicationLifeCycle> Completed;

		public void PerformWorkInAlternateThread(Work work)
		{
			work.BeginInvoke(delegate { }, null);
		}
	}
}