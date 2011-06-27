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
namespace MassTransit.Tests.Services.Timeout
{
	using System;
	using System.Diagnostics;
	using BusConfigurators;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using MassTransit.Saga;
	using MassTransit.Services.Timeout.Messages;
	using MassTransit.Services.Timeout.Server;
	using NUnit.Framework;
	using TestFramework;
	using TextFixtures;

	[TestFixture]
	public class When_scheduling_two_sagas_of_the_same_timeout_id :
		LoopbackTestFixture
	{
		InMemorySagaRepository<TimeoutSaga> _timeoutSagaRepository;

		protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
		{
			base.ConfigureLocalBus(configurator);

			_timeoutSagaRepository = SetupSagaRepository<TimeoutSaga>();

			configurator.Subscribe(s => { s.Saga(_timeoutSagaRepository); });
		}

		[Test]
		public void Should_contain_two_messages_should_be_correlated_by_tag_and_id()
		{
			Guid timeoutId = Guid.NewGuid();

			LocalBus.Publish(new ScheduleTimeout(timeoutId, 10.Seconds(), 1));

			TimeoutSaga firstTimeout = _timeoutSagaRepository.ShouldContainSaga(x => x.TimeoutId == timeoutId && x.Tag == 1);
			firstTimeout.ShouldBeInState(TimeoutSaga.WaitingForTime);

			LocalBus.Publish(new CancelTimeout {CorrelationId = timeoutId, Tag = 1});

			LocalBus.Publish(new ScheduleTimeout(timeoutId, 10.Seconds(), 2));
			TimeoutSaga secondTimeout = _timeoutSagaRepository.ShouldContainSaga(x => x.TimeoutId == timeoutId && x.Tag == 2);
			secondTimeout.ShouldBeInState(TimeoutSaga.WaitingForTime);

			firstTimeout.ShouldBeInState(TimeoutSaga.Completed);

			Trace.WriteLine("Sagas:");
			foreach (TimeoutSaga saga in _timeoutSagaRepository.Where(x => true))
			{
				Trace.WriteLine("Saga: " + saga.CorrelationId + ", TimeoutId: " + saga.TimeoutId + ", Tag: " + saga.Tag +
				                ", State: " + saga.CurrentState.Name);
			}
		}

		[Test]
		public void Should_allow_two_timeouts_to_be_set_simultaneously_for_the_same_id()
		{
			Guid timeoutId = Guid.NewGuid();

			LocalBus.Publish(new ScheduleTimeout(timeoutId, 10.Seconds(), 1));
			LocalBus.Publish(new ScheduleTimeout(timeoutId, 10.Seconds(), 2));

			var one = _timeoutSagaRepository.ShouldContainSaga(s => s.TimeoutId == timeoutId && s.Tag == 1);
			one.ShouldNotBeNull();

			var two = _timeoutSagaRepository.ShouldContainSaga(s => s.TimeoutId == timeoutId && s.Tag == 2);
			two.ShouldNotBeNull();

			one.ShouldBeInState(TimeoutSaga.WaitingForTime);
			two.ShouldBeInState(TimeoutSaga.WaitingForTime);

			Trace.WriteLine("Sagas:");
			foreach (TimeoutSaga saga in _timeoutSagaRepository.Where(x => true))
			{
				Trace.WriteLine("Saga: " + saga.CorrelationId + ", Tag: " + saga.Tag + ", State: " + saga.CurrentState.Name);
			}
		}
	}
}