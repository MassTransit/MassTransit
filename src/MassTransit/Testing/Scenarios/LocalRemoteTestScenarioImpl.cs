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

	public class LocalRemoteTestScenarioImpl :
		EndpointTestScenarioImpl,
		LocalRemoteTestScenario
	{
		bool _disposed;
		IServiceBus _localBus;
		IServiceBus _remoteBus;
		IServiceBus _realLocalBus;
		IServiceBus _realRemoteBus;

		public LocalRemoteTestScenarioImpl(IEndpointFactory endpointFactory)
			: base(endpointFactory)
		{
		}

		public override IServiceBus InputBus
		{
			get { return RemoteBus; }
		}

        public override IServiceBus OutputBus
        {
            get { return InputBus; }
        }

		public IServiceBus LocalBus
		{
			get { return _localBus; }
			set
			{
				_realLocalBus = value;
				_localBus = new ServiceBusTestDecorator(value, this);
			}
		}

		public IServiceBus RemoteBus
		{
			get { return _remoteBus; }
			set
			{
				_realRemoteBus = value;
				_remoteBus = new ServiceBusTestDecorator(value, this);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (RemoteBus != null)
					RemoteBus.Dispose();

				if (LocalBus != null)
					LocalBus.Dispose();

				base.Dispose(true);
			}

			_disposed = true;
		}

		public override IServiceBus GetDecoratedBus(IServiceBus bus)
		{
			if (bus == _realLocalBus)
				return _localBus;

			if(bus == _realRemoteBus)
				return _realRemoteBus;

			return base.GetDecoratedBus(bus);
		}
	}
}