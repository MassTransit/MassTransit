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
	using System;
	using System.Linq;
	using System.Threading;
	using Magnum.DateTimeExtensions;
	using MassTransit.Services.HealthMonitoring.Messages;
	using MassTransit.Services.HealthMonitoring.Server;
	using MassTransit.Services.Timeout.Messages;
	using NUnit.Framework;

	[TestFixture]
	public class HealthService_Specs :
		HealthServiceTestFixture
	{
		private readonly Guid _id = Guid.NewGuid();

		private void MakeSagaDown()
		{
			MakeSagaSuspect();
			var fm = new FutureMessage<TimeoutExpired>();
			RemoteBus.Subscribe<TimeoutExpired>(fm.Set);
			LocalBus.Publish(new TimeoutExpired {CorrelationId = _id, Tag = 2});
			fm.IsAvailable(1.Seconds()).ShouldBeTrue();

			var saga = Repository.Where(x => x.CorrelationId == _id).First();
			saga.CurrentState.ShouldEqual(HealthSaga.Down, "MakeSagaDown failed");
		}


		public void MakeSagaSuspect()
		{
			LocalBus.Publish(new EndpointTurningOn(LocalBus.Endpoint.Uri, 0, _id));
			var fm = new FutureMessage<TimeoutExpired>();
			RemoteBus.Subscribe<TimeoutExpired>(fm.Set);
			Thread.Sleep(500);
			LocalBus.Publish(new TimeoutExpired {CorrelationId = _id, Tag = 1});
			fm.IsAvailable(1.Seconds()).ShouldBeTrue();

			var saga = Repository.Where(x => x.CorrelationId == _id).FirstOrDefault();
			saga.ShouldNotBeNull();
			saga.CurrentState.ShouldEqual(HealthSaga.Suspect, "MakeSagaSuspect failed");
		}

		[Test]
		public void If_a_heartbeat_is_missed_the_saga_should_mark_the_endpoint_suspect()
		{
			MakeSagaSuspect();

			Repository.Where(x => x.CurrentState == HealthSaga.Suspect).Count().ShouldEqual(1);
		}

		[Test]
		public void If_a_saga_is_suspect_a_heartbeat_should_fix()
		{
			MakeSagaSuspect();

			LocalBus.Publish(new Heartbeat(LocalBus.Endpoint.Uri, _id));

			Thread.Sleep(500);
			Repository.Where(x => x.CurrentState == HealthSaga.Healthy).Count().ShouldEqual(1);
		}

		[Test]
		[Ignore("need to improve saga/timeout support")]
		public void If_a_saga_is_suspect_a_pingtimeout_should_make_it_down()
		{
			MakeSagaSuspect();

			var fm = new FutureMessage<TimeoutExpired>();
			RemoteBus.Subscribe<TimeoutExpired>(fm.Set);
			LocalBus.Publish(new TimeoutExpired {CorrelationId = _id, Tag = 2});

			fm.IsAvailable(1.Seconds()).ShouldBeTrue("never got message");

			var saga = Repository.Where(x => x.CorrelationId == _id).First();
			saga.CurrentState.ShouldEqual(HealthSaga.Down);
		}

		[Test]
		public void If_a_saga_is_suspect_a_pong_should_fix()
		{
			MakeSagaSuspect();

			LocalBus.Publish(new Pong(_id, LocalBus.Endpoint.Uri));
			Thread.Sleep(500);
			Repository.Where(x => x.CurrentState == HealthSaga.Healthy).Count().ShouldEqual(1);
		}

		[Test]
		[Ignore("need to improve saga/timeout support")]
		public void If_endpoint_down_a_heartbeat_should_revive()
		{
			MakeSagaDown();
			Repository.Where(x => x.CurrentState == HealthSaga.Down).Count().ShouldEqual(1);
			LocalBus.Publish(new Heartbeat(LocalBus.Endpoint.Uri, _id));
		}

		[Test]
		public void The_HealthClient_should_publish_heartbeats()
		{
			LocalBus.Publish(new EndpointTurningOn(LocalBus.Endpoint.Uri, 1, _id));
			Thread.Sleep(500);

			HealthSaga saga = Repository.Where(x => x.CorrelationId == _id).FirstOrDefault();
			saga.ShouldNotBeNull();
			saga.CurrentState.ShouldEqual(HealthSaga.Healthy);
		}
	}
}