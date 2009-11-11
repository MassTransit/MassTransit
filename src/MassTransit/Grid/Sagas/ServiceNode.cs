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
namespace MassTransit.Grid.Sagas
{
	using System;
	using Magnum.StateMachine;
	using Messages;
	using Saga;

	public class ServiceNode :
		SagaStateMachine<ServiceNode>,
		ISaga
	{
		static ServiceNode()
		{
			Define(() =>
				{
					Correlate(Added)
						.By((saga, message) => saga.ControlUri == message.ControlUri &&
						                       saga.ServiceId == message.ServiceId);

					Correlate(Removed)
						.By((saga, message) => saga.ControlUri == message.ControlUri &&
						                       saga.ServiceId == message.ServiceId);

					RemoveWhen(saga => saga.CurrentState == Completed);

					Initially(
						When(Added)
							.Then((saga, message) =>
								{
									saga.ControlUri = message.ControlUri;
									saga.DataUri = message.DataUri;
									saga.ServiceId = message.ServiceId;
									saga.ServiceName = message.ServiceName;

									saga.SendNotificationMessage<ServiceNodeAdded>();
								})
							.TransitionTo(Active));

					During(Active,
						When(Removed)
							.Then((saga, message) => saga.SendNotificationMessage<ServiceNodeRemoved>())
							.Complete());
				});
		}

		public ServiceNode(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected ServiceNode()
		{
		}

		public static State Initial { get; set; }
		public static State Active { get; set; }
		public static State Completed { get; set; }

		public static Event<AddServiceNode> Added { get; set; }
		public static Event<RemoveServiceNode> Removed { get; set; }

		public Uri ControlUri { get; set; }
		public Uri DataUri { get; set; }
		public Guid ServiceId { get; set; }
		public string ServiceName { get; set; }

		public Guid CorrelationId { get; set; }

		public IServiceBus Bus { get; set; }

		private void SendNotificationMessage<T>()
			where T : AbstractServiceNodeMessage, new()
		{
			var message = new T
				{
					ServiceId = ServiceId,
					ServiceName = ServiceName,
					ControlUri = ControlUri,
					DataUri = DataUri,
				};

			Bus.Endpoint.Send(message);
		}
	}
}