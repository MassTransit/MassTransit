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
	using System.Collections.Generic;
	using System.Linq;
	using log4net;
	using Magnum.StateMachine;
	using Saga;

	public class GridMessageNode :
		SagaStateMachine<GridMessageNode>,
		ISaga
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (GridMessageNode));

		static GridMessageNode()
		{
			Define(() =>
				{
					Correlate(MessageProposedByNode).By((saga, message) => saga.CorrelationId == message.CorrelationId);
					Correlate(MessageAcceptedByNode).By((saga, message) => saga.CorrelationId == message.CorrelationId);
					Correlate(MessageCompletedByNode).By((saga, message) => saga.CorrelationId == message.CorrelationId);
					Correlate(MessageNodeAgreed).By((saga, message) => saga.CorrelationId == message.CorrelationId);

					Initially(
						When(MessageProposedByNode)
							.Call((saga, message) => saga.Initialize(message))
							.Then((saga, message) =>
								{
									_log.InfoFormat("{0} PRPOSE: {1}.{2}:{3}", saga.Bus.Endpoint.Uri,
									                message.CorrelationId, message.BallotId, message.ControlUri);
								})
							.Call((saga, message) => saga.AcceptProposal())
							.TransitionTo(WaitingForAgreement),
						When(MessageAcceptedByNode)
							.Call((saga, message) => saga.Initialize(message))
							.Call((saga, message) => saga.AcceptProposal())
							.TransitionTo(WaitingForAgreement),
						When(MessageNodeAgreed)
							.Call((saga, message) => saga.Initialize(message))
							.TransitionTo(WaitingForCompletion),
						When(MessageCompletedByNode)
							.TransitionTo(Completed)
						);

					During(WaitingForAgreement,
					       When(MessageProposedByNode)
					       	.Call((saga, message) => saga.IgnoreProposal(message)),
					       When(MessageAcceptedByNode)
					       	.Call((saga, message) => saga.ValidateMessageAcceptance(message)),
					       When(MessageNodeAgreed)
					       	.Then((saga, message) =>
					       		{
					       			_log.InfoFormat("{0} AGREED: {1}.{2}:{3}", saga.Bus.Endpoint.Uri,
					       			                message.CorrelationId, message.BallotId, message.ControlUri);
					       		})
					       	.TransitionTo(WaitingForCompletion)
						);

					During(WaitingForCompletion,
					       When(MessageCompletedByNode)
					       	.TransitionTo(Completed)
						);
				});
		}

		public GridMessageNode(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected GridMessageNode()
		{
		}

		public static State Initial { get; set; }
		public static State WaitingForAgreement { get; set; }
		public static State WaitingForCompletion { get; set; }
		public static State Completed { get; set; }

		public static Event<ProposeMessageNode> MessageProposedByNode { get; set; }
		public static Event<AcceptProposedMessageNode> MessageAcceptedByNode { get; set; }
		public static Event<MessageCompleted> MessageCompletedByNode { get; set; }
		public static Event<AcceptedMessageNode> MessageNodeAgreed { get; set; }

		public long BallotId { get; set; }
		public Uri ControlUri { get; set; }
		public Uri DataUri { get; set; }
		public Guid NodeId { get; set; }
		protected IList<QuorumState> Quorum { get; set; }

		public Guid CorrelationId { get; set; }
		public IServiceBus Bus { get; set; }

		private void ValidateMessageAcceptance(GridMessageNodeMessageBase message)
		{
			UpdateQuorumNode(CurrentMessage.Headers.SourceAddress, message.BallotId, message.ControlUri);

			if (AcceptedByAnotherNodeWithoutBallotIncrease(message))
				return;

			if (ValuesDoNotMatchForSameBallotId(message))
				return;

			bool updateRequired = false;
			if (ValuesAreDifferent(message))
			{
				BallotId = message.BallotId;
				ControlUri = message.ControlUri;
				DataUri = message.DataUri;

				updateRequired = true;
			}

			int agreed;
			if(AgreementHasBeenReached(out agreed))
			{
				Bus.Publish(CreateMessage<AcceptedMessageNode>(), c => c.SetSourceAddress(Bus.Endpoint.Uri));
			}
			else
			{
				_log.InfoFormat("{0} ACCEPT: {1}.{2}:{3} ({4} of {5})", Bus.Endpoint.Uri,
				                message.CorrelationId, message.BallotId, message.ControlUri,
								agreed, Quorum.Count);

				if(updateRequired)
					UpdateQuorum<AcceptProposedMessageNode>();
			}
		}

		private bool AgreementHasBeenReached(out int agreed)
		{
			agreed = GetAgreedCount();

			return agreed == Quorum.Count;
		}

		private int GetAgreedCount()
		{
			return Quorum.Where(x => x.BallotId == BallotId && x.Value == ControlUri.ToString()).Count();
		}


		private void UpdateQuorumNode(Uri controlUri, long ballotId, Uri value)
		{
			QuorumState node = Quorum.Where(x => x.ControlUri == controlUri).FirstOrDefault();
			node.BallotId = ballotId;
			node.Value = value.ToString();
		}

		private bool ValuesAreDifferent(GridMessageNodeMessageBase message)
		{
			if (message.ControlUri != ControlUri ||
			    message.DataUri != DataUri ||
			    message.BallotId != BallotId)
				return true;

			return false;
		}

		private bool AcceptedByAnotherNodeWithoutBallotIncrease(GridMessageNodeMessageBase message)
		{
			if (message.BallotId < BallotId)
			{
				_log.InfoFormat("{0} REJECT: {1}.{2}:{3} {4}:{5} {6}", Bus.Endpoint.Uri,
				                message.CorrelationId, message.BallotId, message.ControlUri,
				                BallotId, ControlUri,
				                "Obsolete");

				UpdateNode<AcceptProposedMessageNode>(CurrentMessage.Headers.SourceAddress);
				return true;
			}

			return false;
		}

		private bool ValuesDoNotMatchForSameBallotId(GridMessageNodeMessageBase message)
		{
			if (message.BallotId > BallotId)
				return false;

			if (ControlUri != message.ControlUri || DataUri != message.DataUri)
			{
				_log.InfoFormat("{0} REJECT: {1}.{2}:{3} {4}:{5} {6}", Bus.Endpoint.Uri,
				                message.CorrelationId, message.BallotId, message.ControlUri,
				                BallotId, ControlUri,
				                "Conflict");

				BallotId += GetAgreedCount();
				UpdateQuorum<AcceptProposedMessageNode>();

				return true;
			}

			return false;
		}

		private void Initialize(GridMessageNodeMessageBase message)
		{
			BallotId = message.BallotId;
			ControlUri = message.ControlUri;
			DataUri = message.DataUri;
			NodeId = message.NodeId;
			Quorum = message.Quorum.Select(x => new QuorumState {ControlUri = x}).ToList();

			_log.InfoFormat("{0}   INIT: {1}.{2}:{3} {4}", Bus.Endpoint.Uri,
			                message.CorrelationId, message.BallotId, message.ControlUri,
			                string.Join(",", Quorum.Select(x => x.ControlUri.ToString()).ToArray()));
		}

		private void IgnoreProposal(GridMessageNodeMessageBase message)
		{
			_log.InfoFormat("{0} IGNORE: {1}.{2}:{3} {4}:{5} {6}", Bus.Endpoint.Uri,
			                message.CorrelationId, message.BallotId, message.ControlUri,
			                BallotId, ControlUri,
			                "Accepting");
		}

		private void AcceptProposal()
		{
			UpdateQuorum<AcceptProposedMessageNode>();
		}

		private void RejectProposal()
		{
			UpdateQuorum<RejectProposedMessageNode>();
		}

		private void UpdateNode<T>(Uri uri)
			where T : GridMessageNodeMessageBase, new()
		{
			var message = CreateMessage<T>();
			var endpointFactory = CurrentMessage.Headers.ObjectBuilder.GetInstance<IEndpointFactory>();

			endpointFactory.GetEndpoint(uri).Send(message, c => c.SetSourceAddress(Bus.Endpoint.Uri));
		}


		private void UpdateQuorum<T>() 
			where T : GridMessageNodeMessageBase, new()
		{
			var message = CreateMessage<T>();
			var endpointFactory = CurrentMessage.Headers.ObjectBuilder.GetInstance<IEndpointFactory>();

			Quorum.Each(x =>
				{
					if(x.ControlUri != ControlUri)
						endpointFactory.GetEndpoint(x.ControlUri).Send(message, c => c.SetSourceAddress(Bus.Endpoint.Uri));
				});
		}

		private void InitiateProposal()
		{
			UpdateQuorum<ProposeMessageNode>();
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
					Quorum = Quorum.Select(x => x.ControlUri).ToList(),
				};
		}
	}

	[Serializable]
	public class QuorumState
	{
		public Uri ControlUri { get; set; }
		public string Value { get; set; }
		public long BallotId { get; set; }
	}

	[Serializable]
	public class ProposeMessageNode :
		GridMessageNodeMessageBase
	{
	}

	[Serializable]
	public class AcceptProposedMessageNode :
		GridMessageNodeMessageBase
	{
	}


	[Serializable]
	public class RejectProposedMessageNode :
		GridMessageNodeMessageBase
	{
	}

	[Serializable]
	public class AcceptedMessageNode :
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
		public IList<Uri> Quorum { get; set; }
		public Guid CorrelationId { get; set; }
	}

	[Serializable]
	public class MessageCompleted :
		CorrelatedBy<Guid>
	{
		public Guid CorrelationId { get; set; }
	}
}