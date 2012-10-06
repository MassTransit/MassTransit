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
namespace MassTransit.Tests.Services.HealthMonitoring
{
	using System;
	using Magnum.TestFramework;
	using MassTransit.Services.HealthMonitoring.Messages;
	using MassTransit.Services.HealthMonitoring.Server;
	using MassTransit.Services.Timeout.Messages;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class When_using_the_health_service :
		HealthServiceTestFixture
	{
		void MakeSagaDown()
		{
			MakeSagaSuspect();

			HealthSaga saga = GetSaga();
			LocalBus.ControlBus.Publish(new TimeoutExpired { CorrelationId = saga.CorrelationId, Tag = 2 });

			saga.ShouldNotBeNull();
			saga.ShouldBeInState(HealthSaga.Down);
		}


		public void MakeSagaSuspect()
		{
			LocalBus.ControlBus.Publish(new EndpointCameOnline(Guid.NewGuid(), LocalBus.ControlBus.Endpoint.Address.Uri, LocalBus.Endpoint.Address.Uri,
				0));

			HealthSaga saga = GetSaga();
			saga.ShouldNotBeNull();
			saga.ShouldBeInState(HealthSaga.Healthy);

			LocalBus.ControlBus.Publish(new TimeoutExpired {CorrelationId = saga.CorrelationId, Tag = 1});

			saga.ShouldBeInState(HealthSaga.Suspect);
		}

		HealthSaga GetSaga()
		{
			return HealthSagaRepository.ShouldContainSaga(x => x.ControlUri == LocalBus.ControlBus.Endpoint.Address.Uri);
		}

		[Test]
		public void If_a_heartbeat_is_missed_the_saga_should_mark_the_endpoint_suspect()
		{
			MakeSagaSuspect();

			HealthSaga saga = GetSaga();
			saga.ShouldNotBeNull();
			saga.ShouldBeInState(HealthSaga.Suspect);
		}

		[Test]
		public void If_a_saga_is_suspect_a_heartbeat_should_fix()
		{
			MakeSagaSuspect();

			HealthSaga saga = GetSaga();
			LocalBus.ControlBus.Publish(new Heartbeat(saga.CorrelationId, LocalBus.ControlBus.Endpoint.Address.Uri, LocalBus.Endpoint.Address.Uri, 0));

			saga.ShouldNotBeNull();
			saga.ShouldBeInState(HealthSaga.Healthy);
		}

		[Test]
		public void If_a_saga_is_suspect_a_pingtimeout_should_make_it_down()
		{
			MakeSagaSuspect();

			HealthSaga saga = GetSaga();
			LocalBus.ControlBus.Publish(new TimeoutExpired {CorrelationId = saga.CorrelationId, Tag = 2});

			saga.ShouldNotBeNull();
			saga.ShouldBeInState(HealthSaga.Down);
		}

		[Test]
		public void If_a_saga_is_suspect_a_pong_should_fix()
		{
			MakeSagaSuspect();

			HealthSaga saga = GetSaga();
			LocalBus.ControlBus.Publish(new PingEndpointResponse(saga.CorrelationId, LocalBus.ControlBus.Endpoint.Address.Uri,
				LocalBus.Endpoint.Address.Uri, 0));

			saga.ShouldNotBeNull();
			saga.ShouldBeInState(HealthSaga.Healthy);
		}

		[Test]
		public void If_endpoint_down_a_heartbeat_should_revive()
		{
			MakeSagaDown();

			HealthSaga saga = GetSaga();
			LocalBus.ControlBus.Publish(new Heartbeat(saga.CorrelationId, LocalBus.ControlBus.Endpoint.Address.Uri, LocalBus.Endpoint.Address.Uri, 0));

			saga.ShouldNotBeNull();
			saga.ShouldBeInState(HealthSaga.Healthy);
		}

		[Test]
		public void Should_mark_an_endpoint_suspect_after_timeout()
		{
			MakeSagaSuspect();
		}

		[Test]
		public void Should_publish_heartbeats_to_the_service()
		{
			LocalBus.ControlBus.Publish(new EndpointCameOnline(Guid.NewGuid(), LocalBus.ControlBus.Endpoint.Address.Uri, LocalBus.Endpoint.Address.Uri,
				0));

			HealthSaga saga = GetSaga();
			saga.ShouldNotBeNull();
			saga.ShouldBeInState(HealthSaga.Healthy);
		}
	}
}