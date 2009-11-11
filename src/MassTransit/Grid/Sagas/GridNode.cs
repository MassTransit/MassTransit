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
	using Util;

	public class GridNode :
		SagaStateMachine<GridNode>,
		ISaga
	{
		static GridNode()
		{
			Define(() =>
				{
					Correlate(NodeAvailable)
						.By((saga, message) => saga.ControlUri == message.ControlUri);
					Correlate(NodeDown)
						.By((saga, message) => saga.ControlUri == message.ControlUri);
					Correlate(NodeWorkload)
						.By((saga, message) => saga.ControlUri == message.ControlUri);

					Initially(
						When(NodeAvailable)
							.Then((saga, message) =>
								{
									saga.CopyNodeDetails(message);
									saga.NotifyServiceGridOfNewNode(message);
								})
							.TransitionTo(Available),
						When(NodeDown)
							.Then((saga, message) =>
								{
									saga.CopyNodeDetails(message);
									saga.NotifyServiceGridOfNewNode(message);
								})
							.TransitionTo(Down),
						When(NodeWorkload)
							.Then((saga, message) =>
								{
									saga.CopyNodeDetails(message);
									saga.NotifyServiceGridOfNewNode(message);
									saga.UpdateWorkload(message);
								})
							.TransitionTo(Down)
						);

					During(Available,
						When(NodeDown)
							.TransitionTo(Down),
						When(NodeWorkload)
							.Then((saga, message) => saga.UpdateWorkload(message)));

					During(Down,
						When(NodeAvailable)
							.TransitionTo(Available));
				});
		}

		public GridNode(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected GridNode()
		{
		}

		public static State Initial { get; set; }
		public static State Completed { get; set; }
		public static State Available { get; set; }
		public static State Down { get; set; }

		public static Event<NotifyNodeAvailable> NodeAvailable { get; set; }
		public static Event<NotifyNodeDown> NodeDown { get; set; }
		public static Event<NotifyNodeWorkload> NodeWorkload { get; set; }


		public Uri ControlUri { get; set; }
		public Uri DataUri { get; set; }
		public DateTime LastUpdated { get; set; }
		public DateTime Created { get; set; }
		public int ActiveJobCount { get; set; }
		public int PendingJobCount { get; set; }

		[Indexed]
		public Guid CorrelationId { get; set; }
		public IServiceBus Bus { get; set; }

		private void NotifyServiceGridOfNewNode(NotifyNodeState message)
		{
			Bus.Endpoint.Send(new NotifyNewNodeAvailable
				{
					ControlUri = message.ControlUri,
					DataUri = message.DataUri,
					LastUpdated = message.LastUpdated,
				});
		}

		private void CopyNodeDetails(NotifyNodeState nodeState)
		{
			ControlUri = nodeState.ControlUri;
			DataUri = nodeState.DataUri;
			Created = nodeState.Created;
			LastUpdated = nodeState.LastUpdated;
		}

		private void UpdateWorkload(NotifyNodeWorkload workload)
		{
			ActiveJobCount = workload.ActiveJobCount;
			PendingJobCount = workload.PendingJobCount;
		}
	}
}