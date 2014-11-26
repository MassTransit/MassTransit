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
		IBus _bus;
		bool _disposed;
		IBus _realBus;
	    ISendEndpoint _outputEndpoint;

	    public BusTestScenarioImpl(IEndpointFactory endpointFactory)
			: base(endpointFactory)
		{
		}

		public override IBus InputBus
		{
			get { return Bus; }
		}

        public override ISendEndpoint OutputBus
        {
            get { return _outputEndpoint; }
        }

		public IBus Bus
		{
			get { return _bus; }
			set
			{
				_realBus = value;
			    _bus = value;
			}
		}

		public override IServiceBus GetDecoratedBus(IServiceBus bus)
		{
			return base.GetDecoratedBus(bus);
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
//				if (Bus != null)
//					Bus.Dispose();

				base.Dispose(true);
			}

			_disposed = true;
		}
	}
}