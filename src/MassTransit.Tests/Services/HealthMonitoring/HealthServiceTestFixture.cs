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
namespace MassTransit.Tests.Services.HealthMonitoring
{
	using System.Threading;
	using MassTransit.Saga;
	using MassTransit.Services.HealthMonitoring;
	using MassTransit.Services.HealthMonitoring.Messages;
	using MassTransit.Services.HealthMonitoring.Server;
	using MassTransit.Services.Timeout.Messages;
	using MassTransit.Transports.Loopback;
	using NUnit.Framework;
	using TextFixtures;
	using TestFramework;

	[TestFixture]
	public class HealthServiceTestFixture :
		SubscriptionServiceTestFixture<LoopbackTransportFactory>
	{
		private ISagaRepository<HealthSaga> _healthSagaHealthSagaRepository;
		public HealthService HealthService { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			SetupHealthService();
		}

		private void SetupHealthService()
		{
			_healthSagaHealthSagaRepository = SetupSagaRepository<HealthSaga>();

			HealthService = new HealthService(RemoteBus, _healthSagaHealthSagaRepository);

			HealthService.Start();

			Thread.Sleep(500);

			LocalBus.ControlBus.ShouldHaveRemoteSubscriptionFor<EndpointCameOnline>();
			LocalBus.ControlBus.ShouldHaveRemoteSubscriptionFor<Heartbeat>();
			LocalBus.ControlBus.ShouldHaveRemoteSubscriptionFor<EndpointWentOffline>();
			LocalBus.ControlBus.ShouldHaveRemoteSubscriptionFor<TimeoutExpired>();
			LocalBus.ControlBus.ShouldHaveRemoteSubscriptionFor<PingEndpointResponse>();
		}

		public ISagaRepository<HealthSaga> HealthSagaRepository
		{
			get { return _healthSagaHealthSagaRepository; }
		}

		protected override void TeardownContext()
		{
			HealthService.Stop();
			HealthService.Dispose();
			HealthService = null;

			base.TeardownContext();
		}
	}
}