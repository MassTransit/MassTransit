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
namespace MassTransit.Services.HealthMonitoring.Server
{
	using System;
	using Logging;
	using Magnum.Extensions;
	using Magnum.StateMachine;
	using Messages;
	using Saga;
	using Timeout.Messages;
	using Util;

	public class HealthSaga :
		SagaStateMachine<HealthSaga>,
		ISaga
	{
		static readonly ILog _log = Logger.Get(typeof (HealthSaga));

	    static HealthSaga()
		{
			Define(() =>
				{
					Correlate(EndpointComesOnline).By((saga, message) => saga.ControlUri == message.ControlUri);
					Correlate(EndpointHeartBeats).By((saga, message) => saga.ControlUri == message.ControlUri);
					Correlate(EndpointGoesOffline).By((saga, message) => saga.ControlUri == message.ControlUri);
					Correlate(EndpointRespondsToPing).By((saga, message) => saga.ControlUri == message.ControlUri);

					Anytime(
						When(EndpointComesOnline)
							.Call((saga, message) => Initialize(saga, message))
							.Then(saga => saga.NotifyEndpointIsHealthy())
							.TransitionTo(Healthy));

					Initially(
						When(EndpointHeartBeats)
							.Call((saga, message) => Initialize(saga, message))
							.Then(saga => saga.NotifyEndpointIsHealthy())
							.TransitionTo(Healthy)
						);

					During(Healthy,
						When(EndpointHeartBeats)
							.Then((saga, message) => saga.ResetHeartbeatTimeout())
							.Then(saga => saga.NotifyEndpointIsHealthy()),
						When(TimeoutExpires).Where(msg => msg.Tag == (int) Timeouts.HeartBeatTimeout)
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
						When(TimeoutExpires).Where(msg => msg.Tag == (int) Timeouts.PingTimeout)
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

		[UsedImplicitly]
		public HealthSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		[UsedImplicitly]
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

		public  DateTime LastHeartbeat { get; set; }
		public  Uri ControlUri { get; set; }
		public  Uri DataUri { get; set; }
		public  int HeartbeatIntervalInSeconds { get; set; }

	    public  Guid CorrelationId { get; set; }

	    public  IServiceBus Bus { get; set; }

		void NotifyEndpointIsDown()
		{
			Bus.Publish(new EndpointIsDown(CorrelationId, ControlUri, DataUri, HeartbeatIntervalInSeconds, LastHeartbeat,
				Down.Name));
		}

		void NotifyEndpointIsSuspect()
		{
			Bus.Publish(new EndpointIsSuspect(CorrelationId, ControlUri, DataUri, HeartbeatIntervalInSeconds, LastHeartbeat,
				Suspect.Name));
		}

		void NotifyEndpointIsOffline()
		{
			Bus.Publish(new EndpointIsOffline(CorrelationId, ControlUri, DataUri, HeartbeatIntervalInSeconds, LastHeartbeat,
				"Off Line"));
		}

		void NotifyEndpointIsHealthy()
		{
			Bus.Publish(new EndpointIsHealthy(CorrelationId, ControlUri, DataUri, HeartbeatIntervalInSeconds, LastHeartbeat,
				Healthy.Name));
		}

		void PingUnresponsiveEndpoint()
		{
			Bus.Publish(new ScheduleTimeout(CorrelationId, HeartbeatIntervalInSeconds.Seconds(), (int) Timeouts.PingTimeout));

			Bus.GetEndpoint(ControlUri)
				.Send(new PingEndpoint(CorrelationId));
		}

		void ResetHeartbeatTimeout()
		{
			LastHeartbeat = DateTime.UtcNow;

			TimeSpan timeoutIn = TimeSpan.FromSeconds((HeartbeatIntervalInSeconds*3)/2);

			Bus.Publish(new ScheduleTimeout(CorrelationId, timeoutIn, (int) Timeouts.HeartBeatTimeout));
		}

		static void Initialize(HealthSaga saga, EndpointMessageBase message)
		{
			saga.ControlUri = message.ControlUri;
			saga.DataUri = message.DataUri;
			saga.HeartbeatIntervalInSeconds = message.HeartbeatIntervalInSeconds;

			saga.ResetHeartbeatTimeout();
		}

		enum Timeouts
		{
			HeartBeatTimeout = 1,
			PingTimeout = 2
		}
	}
}