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
namespace MassTransit.Testing.TestContexts
{
	using System;
	using Transports;

	public class BusTestContext :
		IBusTestContext
	{
		IServiceBus _bus;
		bool _disposed;
		IEndpointTestContext _testContext;

		public BusTestContext(IEndpointTestContext testContext, IServiceBus bus)
		{
			_testContext = testContext;
			_bus = bus;
		}

		public IEndpointCache EndpointCache
		{
			get { return _testContext.EndpointCache; }
		}

		public IEndpointFactory EndpointFactory
		{
			get { return _testContext.EndpointFactory; }
		}

		public IServiceBus Bus
		{
			get { return _bus; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (_bus != null)
					_bus.Dispose();

				_testContext.Dispose();
			}

			_disposed = true;
		}

		~BusTestContext()
		{
			Dispose(false);
		}
	}
}