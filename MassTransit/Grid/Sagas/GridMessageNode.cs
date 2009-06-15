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
	using Saga;

	public class GridMessageNode :
		SagaStateMachine<GridMessageNode>,
		ISaga
	{
		static GridMessageNode()
		{
			Define(() =>
				{
					Initially(
						When(MessageReceivedByNode)
							.Call((saga, message) => saga.Initialize(message))
							.Call((saga, message) => saga.InitiateProposal())
							.TransitionTo(WaitingForAgreement),
						When(MessageProposedByNode)
							.Call((saga, message) => saga.Initialize(message))
							.Call((saga, message) => saga.AcceptProposal())
							.TransitionTo(WaitingForCompletion),
						When(MessageAcceptedByNode)
							.Call((saga, message) => saga.Initialize(message))
							.TransitionTo(WaitingForCompletion)
						);

					During(WaitingForAgreement,
					       When(MessageAcceptedByNode)
					       	.Call((saga, message) => saga.ValidateMessageAcceptance(message))
					       	.TransitionTo(WaitingForCompletion)
						);

					During(WaitingForCompletion,
					       When(MessageCompletedByNode)
					       	.TransitionTo(Completed)
						);
				});
		}

		public static State Initial { get; set; }
		public static State WaitingForAgreement { get; set; }
		public static State WaitingForCompletion { get; set; }
		public static State Completed { get; set; }

		public static Event<NewMessageReceived> MessageReceivedByNode { get; set; }
		public static Event<ProposeMessageNode> MessageProposedByNode { get; set; }
		public static Event<AcceptProposedMessageNode> MessageAcceptedByNode { get; set; }
		public static Event<MessageCompleted> MessageCompletedByNode { get; set; }

		public long BallotId { get; set; }
		public Uri ControlUri { get; set; }
		public Uri DataUri { get; set; }
		public Guid NodeId { get; set; }

		public Guid CorrelationId { get; set; }
		public IServiceBus Bus { get; set; }

		private void ValidateMessageAcceptance(AcceptProposedMessageNode message)
		{
		}

		private void Initialize(GridMessageNodeMessageBase message)
		{
			BallotId = message.BallotId;
			ControlUri = message.ControlUri;
			DataUri = message.DataUri;
			NodeId = message.NodeId;
		}

		private void AcceptProposal()
		{
			Bus.Publish(CreateMessage<AcceptProposedMessageNode>());
		}

		private void InitiateProposal()
		{
			Bus.Publish(CreateMessage<ProposeMessageNode>());
		}

		private T CreateMessage<T>()
			where T : GridMessageNodeMessageBase, new()
		{
			return new T
				{
					BallotId = BallotId,
					CorrelationId = CorrelationId,
					ControlUri = ControlUri,
					DataUri = DataUri,
					NodeId = NodeId,
				};
		}
	}

	[Serializable]
	public class ProposeMessageNode :
		GridMessageNodeMessageBase
	{
	}

	[Serializable]
	public class MessageCompleted :
		CorrelatedBy<Guid>
	{
		public Guid CorrelationId { get; set; }
	}

	[Serializable]
	public class AcceptProposedMessageNode :
		GridMessageNodeMessageBase
	{
	}

	[Serializable]
	public class GridMessageNodeMessageBase :
		CorrelatedBy<Guid>
	{
		public long BallotId { get; set; }
		public Guid NodeId { get; set; }
		public Uri ControlUri { get; set; }
		public Uri DataUri { get; set; }
		public Guid CorrelationId { get; set; }
	}

	[Serializable]
	public class NewMessageReceived :
		GridMessageNodeMessageBase
	{
	}
}