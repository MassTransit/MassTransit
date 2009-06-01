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
					Correlate(EndpointHeartBeats).By((saga, message) => saga.CorrelationId == message.CorrelationId);

					Initially(
						When(EndpointComesOnline)
							.Then((saga, message) =>
								{
									_log.DebugFormat("Endpoint came online: {0} ({1})", message.DataUri, message.ControlUri);

									InitializeSagaFromMessage(saga, message.ControlUri, message.DataUri, message.HeartbeatIntervalInSeconds);
								})
							.Then(saga => saga.NotifyEndpointIsHealthy())
							.TransitionTo(Healthy) /*,
						When(EndpointBreathes)
							.Then((saga, message) =>
								{
									saga.EndpointAddress = message.EndpointAddress;
									saga.TimeBetweenBeatsInSeconds = 10;
									saga.MarkBeat();
									saga.ScheduleTimeoutForHeartbeat();
								})
							.Then(StatusChange)
							.TransitionTo(Healthy)*/
						);

					During(Healthy,
						When(EndpointHeartBeats)
							.Then((saga, message) => saga.ResetHeartbeatTimeout()),
						When(TimeoutExpires).And(msg => msg.Tag == (int) Timeouts.HeartBeatTimeout)
							.Then((saga, message) => saga.PingUnresponsiveEndpoint())
							.Then(saga => saga.NotifyEndpointIsSuspect())
							.TransitionTo(Suspect),
						When(EndpointGoesOffline)
							.Then(saga => saga.NotifyEndpointIsOffline())
							.TransitionTo(Completed));

					During(Suspect,
						When(EndpointHeartBeats)
							.Then(saga => saga.NotifyEndpointIsHealthy())
							.TransitionTo(Healthy),
						When(EndpointRespondsToPing)
							.Then(saga => saga.NotifyEndpointIsHealthy())
							.TransitionTo(Healthy),
						When(TimeoutExpires).And(msg => msg.Tag == (int) Timeouts.PingTimeout)
							.Then((saga, message) => saga.NotifyEndpointIsDown())
							.TransitionTo(Down),
						When(EndpointGoesOffline)
							.Then(saga => saga.NotifyEndpointIsOffline())
							.TransitionTo(Completed));

					During(Down,
						When(EndpointHeartBeats)
							.Then(saga => saga.NotifyEndpointIsHealthy())
							.TransitionTo(Healthy),
						When(EndpointGoesOffline)
							.Then(saga => saga.NotifyEndpointIsOffline())
							.TransitionTo(Completed));
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

		public static Event<EndpointCameOnline> EndpointComesOnline { get; set; }
		public static Event<Heartbeat> EndpointHeartBeats { get; set; }
		public static Event<EndpointWentOffline> EndpointGoesOffline { get; set; }
		public static Event<TimeoutExpired> TimeoutExpires { get; set; }
		public static Event<PingEndpointResponse> EndpointRespondsToPing { get; set; }

		public virtual DateTime LastHeartbeat { get; set; }
		public virtual Uri ControlUri { get; set; }
		public virtual Uri DataUri { get; set; }
		public virtual int HeartbeatIntervalInSeconds { get; set; }

		public virtual Guid CorrelationId { get; set; }
		public virtual IServiceBus Bus { get; set; }

		private void NotifyEndpointIsDown()
		{
			Bus.Publish(new EndpointIsDown(CorrelationId, ControlUri, DataUri, HeartbeatIntervalInSeconds, LastHeartbeat, Down.Name));
		}

		private void NotifyEndpointIsSuspect()
		{
			Bus.Publish(new EndpointIsSuspect(CorrelationId, ControlUri, DataUri, HeartbeatIntervalInSeconds, LastHeartbeat, Suspect.Name));
		}

		private void NotifyEndpointIsOffline()
		{
			Bus.Publish(new EndpointIsOffline(CorrelationId, ControlUri, DataUri, HeartbeatIntervalInSeconds, LastHeartbeat, "Off Line"));
		}

		private void NotifyEndpointIsHealthy()
		{
			Bus.Publish(new EndpointIsHealthy(CorrelationId, ControlUri, DataUri, HeartbeatIntervalInSeconds, LastHeartbeat, Healthy.Name));
		}

		private void PingUnresponsiveEndpoint()
		{
			Bus.Publish(new ScheduleTimeout(CorrelationId, HeartbeatIntervalInSeconds.Seconds(), (int) Timeouts.PingTimeout));

			// TODO we need to inject the object builder into the class I think
			HealthService.Builder
				.GetInstance<IEndpointFactory>()
				.GetEndpoint(ControlUri)
				.Send(new PingEndpoint(CorrelationId));
		}

		private void ResetHeartbeatTimeout()
		{
			LastHeartbeat = DateTime.UtcNow;

			Bus.Publish(new ScheduleTimeout(CorrelationId, TimeSpan.FromSeconds(HeartbeatIntervalInSeconds*2), (int) Timeouts.HeartBeatTimeout));
		}

		private void MarkBeat()
		{
			LastHeartbeat = DateTime.Now;
		}

		private static void InitializeSagaFromMessage(HealthSaga saga, Uri controlUri, Uri dataUri, int heartbeatIntervalInSeconds)
		{
			saga.ControlUri = controlUri;
			saga.DataUri = dataUri;
			saga.HeartbeatIntervalInSeconds = heartbeatIntervalInSeconds;

			saga.ResetHeartbeatTimeout();
		}

		private enum Timeouts
		{
			HeartBeatTimeout = 1,
			PingTimeout = 2
		}
	}
}