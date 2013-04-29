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
namespace MassTransit.Testing.Scenarios
{
	using TestDecorators;
	using Transports;

	public class BusTestScenarioImpl :
		EndpointTestScenarioImpl,
		BusTestScenario
	{
		IServiceBus _bus;
		bool _disposed;
		IServiceBus _realBus;

		public BusTestScenarioImpl(IEndpointFactory endpointFactory)
			: base(endpointFactory)
		{
		}

		public override IServiceBus InputBus
		{
			get { return Bus; }
		}

        public override IServiceBus OutputBus
        {
            get { return Bus; }
        }

		public IServiceBus Bus
		{
			get { return _bus; }
			set
			{
				_realBus = value;
				_bus = new ServiceBusTestDecorator(value, this);
			}
		}

		public override IServiceBus GetDecoratedBus(IServiceBus bus)
		{
			if (_realBus == bus)
				return _bus;

			return base.GetDecoratedBus(bus);
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (Bus != null)
					Bus.Dispose();

				base.Dispose(true);
			}

			_disposed = true;
		}
	}
}