// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Distributor
{
	using System;
	using BusConfigurators;

	public class ServiceInstance :
		IDisposable
	{
		volatile bool _disposed;

		public ServiceInstance(string name, string subscriptionServiceEndpointAddress, Action<ServiceBusConfigurator> configurator)
		{
			DataBus = ServiceBusFactory.New(x =>
				{
					x.UseSubscriptionService(subscriptionServiceEndpointAddress);
					x.ReceiveFrom(name);
					x.UseControlBus();
					x.SetConcurrentConsumerLimit(5);

					configurator(x);
				});
		}

		public IServiceBus DataBus { get; private set; }

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			DataBus.Dispose();
			DataBus = null;

			_disposed = true;
		}
	}
}