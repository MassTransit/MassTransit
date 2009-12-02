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
namespace MassTransit.Tests.Distributor
{
	using System;
	using Configuration;
	using MassTransit.Services.Subscriptions.Configuration;
	using Rhino.Mocks;

	public class ServiceInstance :
		IDisposable
	{
		private volatile bool _disposed;

		public ServiceInstance(string name, IEndpointFactory endpointFactory, string subscriptionServiceEndpointAddress, Action<IServiceBusConfigurator> configurator)
		{
			ObjectBuilder = MockRepository.GenerateMock<IObjectBuilder>();
			ObjectBuilder.Stub(x => x.GetInstance<IEndpointFactory>()).Return(endpointFactory);

			ControlBus = ControlBusConfigurator.New(x =>
				{
					x.SetObjectBuilder(ObjectBuilder);
					x.ReceiveFrom(name + "_control");

					x.PurgeBeforeStarting();
				});

			DataBus = ServiceBusConfigurator.New(x =>
				{
					x.SetObjectBuilder(ObjectBuilder);
					x.ConfigureService<SubscriptionClientConfigurator>(y =>
						{
							// setup endpoint
							y.SetSubscriptionServiceEndpoint(subscriptionServiceEndpointAddress);
						});
					x.ReceiveFrom(name);
					x.UseControlBus(ControlBus);
					x.SetConcurrentConsumerLimit(1);

					configurator(x);
				});
		}

		public IObjectBuilder ObjectBuilder { get; private set; }
		public IControlBus ControlBus { get; private set; }
		public IServiceBus DataBus { get; private set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			DataBus.Dispose();
			DataBus = null;

			ControlBus.Dispose();
			ControlBus = null;

			_disposed = true;
		}

		~ServiceInstance()
		{
			Dispose(false);
		}
	}
}