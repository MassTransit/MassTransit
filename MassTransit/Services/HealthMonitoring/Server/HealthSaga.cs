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
namespace MassTransit.Services.HealthMonitoring.Server
{
	using System;
	using log4net;
	using Magnum.DateTimeExtensions;
	using Magnum.StateMachine;
	using Messages;
	using Microsoft.Practices.ServiceLocation;
	using Saga;
	using Timeout.Messages;

	public class HealthSaga :
		SagaStateMachine<HealthSaga>,
		ISaga
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (HealthSaga));

		static HealthSaga()
		{
			Define(() =>
				{
					Correlate(EndpointBreathes).By((saga, message) => saga.CorrelationId == message.CorrelationId);

					Initially(
						When(EndpointDetected)
							.Then((saga, message) =>
								{
									_log.DebugFormat("Endpoint '{0}' detected", message.EndpointAddress);

									//store stuff
									saga.EndpointAddress = message.EndpointAddress;
									saga.TimeBetweenBeatsInSeconds = message.TimeBetweenBeatsInSeconds;
									saga.MarkBeat();
									saga.ScheduleTimeoutForHeartbeat();
								})
							.Then(StatusChange)
							.TransitionTo(Healthy),
						When(EndpointBreathes)
							.Then((saga, message) =>
								{
									saga.EndpointAddress = message.EndpointAddress;
									saga.TimeBetweenBeatsInSeconds = 10;
									saga.MarkBeat();
									saga.ScheduleTimeoutForHeartbeat();
								})
							.Then(StatusChange)
							.TransitionTo(Healthy));

					During(Healthy,
						When(EndpointBreathes)
							.Then((saga, message) =>
								{
									saga.MarkBeat();
									saga.ScheduleTimeoutForHeartbeat();
								})
							.Then(StatusChange),
						When(TimeoutExpired).And(msg => msg.Tag == (int) Timeouts.HeartBeatTimeout)
							.Then((saga, message) =>
								{
									saga.ScheduleTimeoutForPing();

									ServiceLocator.Current
										.GetInstance<IEndpointFactory>()
										.GetEndpoint(saga.EndpointAddress)
										.Send(new Ping(saga.CorrelationId));
								})
							.Then(StatusChange)
							.TransitionTo(Suspect));


					During(Suspect,
						When(EndpointBreathes)
							.Then(StatusChange)
							.TransitionTo(Healthy),
						When(SuspectRespondsToPing)
							.Then(StatusChange)
							.TransitionTo(Healthy),
						When(PingTimesout).And((msg) => msg.Tag == (int) Timeouts.PingTimeout)
							.Then((saga, message) => { saga.Bus.Publish(new DownEndpoint(saga.EndpointAddress)); })
							.Then(StatusChange)
							.TransitionTo(Down));

					During(Down,
						When(EndpointBreathes)
							.Then(StatusChange)
							.TransitionTo(Healthy));

					//Anytime(EndpointPoweringDown).Complete();
				});
		}

		public HealthSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public HealthSaga()
		{
		}

		public static State Initial { get; set; }
		public static State Healthy { get; set; }
		public static State Suspect { get; set; }
		public static State Down { get; set; }
		public static State Completed { get; set; }

		public static Event<EndpointTurningOn> EndpointDetected { get; set; }
		public static Event<Heartbeat> EndpointBreathes { get; set; }
		public static Event<EndpointTurningOff> EndpointPoweringDown { get; set; }
		public static Event<TimeoutExpired> TimeoutExpired { get; set; }
		public static Event<Pong> SuspectRespondsToPing { get; set; }
		public static Event<TimeoutExpired> PingTimesout { get; set; }

		public virtual DateTime LastHeartbeat { get; set; }
		public virtual Uri EndpointAddress { get; set; }
		public virtual int TimeBetweenBeatsInSeconds { get; set; }

		public virtual Guid CorrelationId { get; set; }
		public virtual IServiceBus Bus { get; set; }

		private bool HasExpiredAccordingToPolicy()
		{
			int actualDuration = DateTime.Now.Subtract(LastHeartbeat).Seconds;
			return (actualDuration/2) > TimeBetweenBeatsInSeconds;
		}

		private void ScheduleTimeoutForHeartbeat()
		{
			int timeout = TimeBetweenBeatsInSeconds*2;

			Bus.Publish(new ScheduleTimeout(CorrelationId, timeout.Seconds(), (int)Timeouts.HeartBeatTimeout));
		}

		private void ScheduleTimeoutForPing()
		{
			int timeout = 5;

			Bus.Publish(new ScheduleTimeout(CorrelationId, timeout.Seconds(), (int)Timeouts.PingTimeout));
		}

		private void MarkBeat()
		{
			LastHeartbeat = DateTime.Now;
		}

		private static void StatusChange(HealthSaga saga)
		{
			saga.Bus.Publish(new StatusChange());
		}

		private enum Timeouts
		{
			HeartBeatTimeout = 1,
			PingTimeout = 2
		}
	}
}