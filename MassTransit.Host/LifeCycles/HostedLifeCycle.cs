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
	using Actions;
	using Microsoft.Practices.ServiceLocation;
	using ServiceBus;

    /// <summary>
    /// Base class to handle the host lifecycle common junk
    /// </summary>
	public abstract class HostedLifeCycle :
		IApplicationLifeCycle
	{
	    private readonly IServiceLocator _serviceLocator;

		protected HostedLifeCycle(IServiceLocator serviceLocator)
		{
		    _serviceLocator = serviceLocator;
		}

	    public void Initialize()
		{
			foreach (IHostedService hs in _serviceLocator.GetAllInstances<IHostedService>())
			{
				hs.Start();
			}
		}

		public abstract void Start();

		public abstract void Stop();

		public virtual NamedAction DefaultAction
		{
			get { return NamedAction.Console; }
		}

		public void Dispose()
		{
            foreach (IHostedService hs in _serviceLocator.GetAllInstances<IHostedService>())
			{
				hs.Stop();
			}

			foreach (IServiceBus bus in _serviceLocator.GetAllInstances<IServiceBus>())
			{
				bus.Dispose();
			}

			if (Completed != null)
			{
				Action<IApplicationLifeCycle> handler = Completed;
				handler(this);
			}
		}

        public IServiceLocator ServiceLocator
        {
            get { return _serviceLocator; }
        }

        //TODO: WTF is this (and I wrote it!)
		public event Action<IApplicationLifeCycle> Completed;
	}
}