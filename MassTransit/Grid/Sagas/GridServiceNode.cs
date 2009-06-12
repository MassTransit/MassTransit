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

	public class GridServiceNode :
		SagaStateMachine<GridServiceNode>,
		ISaga
	{
		static GridServiceNode()
		{
			Define(() =>
				{
					Correlate(ServiceAddedToNode)
						.By((saga, message) => saga.ControlUri == message.ControlUri &&
						                       saga.ServiceId == message.ServiceId);
					Correlate(ServiceRemovedFromNode)
						.By((saga, message) => saga.ControlUri == message.ControlUri &&
						                       saga.ServiceId == message.ServiceId);

					// TODO Add key generation script as well
					// .CreateUsingId(() => CombGuid.Generate())
					// .CreateUsingId(message => message.ServiceId)
					// would also be used to define the creation policy if this message if received
					// and no saga exists


					Initially(
						When(ServiceAddedToNode)
							.Then((saga, message) =>
								{
									saga.ControlUri = message.ControlUri;
									saga.DataUri = message.DataUri;
									saga.ServiceId = message.ServiceId;
									saga.ServiceName = message.ServiceName;

									saga.NotifyServiceAddedToNode();
								})
							.TransitionTo(Active));

					During(Active,
						When(ServiceRemovedFromNode)
							.Then((saga, message) => saga.NotifyServiceRemovedFromNode())
							.Complete());

					During(Completed,
						When(ServiceAddedToNode)
							.Then((saga, message) =>
								{
									saga.DataUri = message.DataUri;
									saga.ServiceName = message.ServiceName;

									saga.NotifyServiceAddedToNode();
								})
							.TransitionTo(Active));
				});
		}

		public GridServiceNode(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected GridServiceNode()
		{
		}

		public static State Initial { get; set; }
		public static State Active { get; set; }
		public static State Completed { get; set; }

		public static Event<AddGridServiceToNode> ServiceAddedToNode { get; set; }
		public static Event<RemoveGridServiceFromNode> ServiceRemovedFromNode { get; set; }

		public Uri ControlUri { get; set; }
		public Uri DataUri { get; set; }
		public Guid ServiceId { get; set; }
		public string ServiceName { get; set; }

		public Guid CorrelationId { get; set; }
		public IServiceBus Bus { get; set; }

		private void NotifyServiceAddedToNode()
		{
			var message = CreateMessage<GridServiceAddedToNode>();

			Bus.Endpoint.Send(message);
		}

		private void NotifyServiceRemovedFromNode()
		{
			var message = CreateMessage<GridServiceRemovedFromNode>();

			Bus.Endpoint.Send(message);
		}

		private T CreateMessage<T>()
			where T : GridServiceMessageBase, new()
		{
			return new T
				{
					ServiceId = ServiceId,
					ServiceName = ServiceName,
					ControlUri = ControlUri,
					DataUri = DataUri,
				};
		}
	}
}