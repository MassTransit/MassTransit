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
	using Messages;
	using Saga;
	using Util;

	public class ServiceMessage :
		SagaStateMachine<ServiceMessage>,
		ISaga
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (ServiceMessage));

		static ServiceMessage()
		{
			Define(() =>
				{
					Correlate(NodeProposed).By((saga, message) => saga.CorrelationId == message.CorrelationId);
					Correlate(NodeAccepted).By((saga, message) => saga.CorrelationId == message.CorrelationId);
					Correlate(NodeCompletedMessage).By((saga, message) => saga.CorrelationId == message.CorrelationId);
					Correlate(NodeAgreed).By((saga, message) => saga.CorrelationId == message.CorrelationId);

					RemoveWhen(saga => saga.CurrentState == Completed);

					Initially(
						When(NodeProposed)
							.Call((saga, message) => saga.Initialize(message))
							.Call((saga,message) => saga.LogAction("PRPOSE", message))
							.Call((saga, message) => saga.AcceptProposal())
							.TransitionTo(WaitingForAgreement),
						When(NodeAccepted)
							.Call((saga, message) => saga.Initialize(message))
							.Call((saga, message) => saga.LogAction("ACCEPT", message))
							.Call((saga, message) => saga.AcceptProposal())
							.TransitionTo(WaitingForAgreement),
						When(NodeAgreed)
							.Call((saga, message) => saga.Initialize(message))
							.Call(saga => saga.PrimeTheBusIfWeAreAssigned())
							.TransitionTo(WaitingForCompletion),
						When(NodeCompletedMessage)
							.TransitionTo(WaitingForReceive)
						);

					During(WaitingForAgreement,
						When(NodeProposed)
							.Call((saga, message) => saga.IgnoreProposal(message)),
						When(NodeAccepted)
							.Call((saga, message) => saga.ValidateMessageAcceptance(message)),
						When(NodeAgreed)
							.Call((saga, message) => saga.LogAction("AGREED", message))
							.Call(saga => saga.PrimeTheBusIfWeAreAssigned())
							.TransitionTo(WaitingForCompletion)
						);

					During(WaitingForCompletion,
						When(NodeCompletedMessage)
							.TransitionTo(WaitingForReceive)
						);

					During(WaitingForReceive,
						When(NodeReceivedMessage)
							.TransitionTo(Completed));
				});
		}

		public ServiceMessage(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected ServiceMessage()
		{
		}

		public static State Initial { get; set; }
		public static State WaitingForAgreement { get; set; }
		public static State WaitingForCompletion { get; set; }
		public static State WaitingForReceive { get; set; }
		public static State Completed { get; set; }

		public static Event<ProposeServiceMessage> NodeProposed { get; set; }
		public static Event<AcceptServiceMessageProposal> NodeAccepted { get; set; }
		public static Event<ServiceMessageAgreed> NodeAgreed { get; set; }
		public static Event<ServiceMessageCompleted> NodeCompletedMessage { get; set; }
		public static Event<ServiceMessageReceived> NodeReceivedMessage { get; set; }

		public long BallotId { get; set; }
		public Uri ControlUri { get; set; }
		public Uri DataUri { get; set; }
		public IList<QuorumState> Quorum { get; set; }

		[Indexed]
		public Guid CorrelationId { get; set; }

		public IServiceBus Bus { get; set; }

		private void PrimeTheBusIfWeAreAssigned()
		{
			// this is done to force a waiting receive on the data bus to continue 
			// if it is waiting at the end of the enumeration to get more messages until a timeout

			if (ControlUri == Bus.Endpoint.Uri)
				Bus.Endpoint.Send(new NullMessage());
		}

		private void LogAction(string action, AbstractServiceMessageMessage message)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("{0} {1}: {2}.{3}:{4}", Bus.Endpoint.Uri, action, message.CorrelationId, message.BallotId, message.ControlUri);
		}

		private void LogAction(string action, AbstractServiceMessageMessage message, int agreed)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("{0} {1}: {2}.{3}:{4} ({5}/{6})", Bus.Endpoint.Uri, action, message.CorrelationId, message.BallotId,
					message.ControlUri, agreed, Quorum.Count);
		}

		private void LogAction(string action, AbstractServiceMessageMessage message, string extra)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("{0} {1}: {2}.{3}:{4} ({5})", Bus.Endpoint.Uri, action, message.CorrelationId, message.BallotId,
					message.ControlUri, extra);
		}

		// TODO manage Quorum using a Quorum class outside of this saga

		private void ValidateMessageAcceptance(AbstractServiceMessageMessage message)
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
			if (AgreementHasBeenReached(out agreed))
			{
				Bus.Publish(CreateMessage<ServiceMessageAgreed>(), c => c.SetSourceAddress(Bus.Endpoint.Uri));
			}
			else
			{
				LogAction("ACCEPT", message, agreed);

				if (updateRequired)
					UpdateQuorum<AcceptServiceMessageProposal>();
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

		private bool ValuesAreDifferent(AbstractServiceMessageMessage message)
		{
			if (message.ControlUri != ControlUri ||
			    message.DataUri != DataUri ||
			    message.BallotId != BallotId)
				return true;

			return false;
		}

		private bool AcceptedByAnotherNodeWithoutBallotIncrease(AbstractServiceMessageMessage message)
		{
			if (message.BallotId < BallotId)
			{
				LogAction("REJECT", message, "obsolete");

				UpdateNode<AcceptServiceMessageProposal>(CurrentMessage.Headers.SourceAddress);
				return true;
			}

			return false;
		}

		private bool ValuesDoNotMatchForSameBallotId(AbstractServiceMessageMessage message)
		{
			if (message.BallotId > BallotId)
				return false;

			if (ControlUri != message.ControlUri || DataUri != message.DataUri)
			{
				LogAction("REJECT", message, "conflict");

				BallotId += GetAgreedCount();
				UpdateQuorum<AcceptServiceMessageProposal>();

				return true;
			}

			return false;
		}

		private void Initialize(AbstractServiceMessageMessage message)
		{
			BallotId = message.BallotId;
			ControlUri = message.ControlUri;
			DataUri = message.DataUri;
			Quorum = message.Quorum.Select(x => new QuorumState {ControlUri = x}).ToList();

			LogAction("  INIT", message,
					"Quorum = " + string.Join(",", Quorum.Select(x => x.ControlUri.ToString()).ToArray()));
		}

		private void IgnoreProposal(AbstractServiceMessageMessage message)
		{
			LogAction("IGNORE", message, "accepting");
		}

		private void AcceptProposal()
		{
			UpdateQuorum<AcceptServiceMessageProposal>();
		}

		private void RejectProposal()
		{
			UpdateQuorum<RejectServiceMessageProposal>();
		}

		private void UpdateNode<T>(Uri uri)
			where T : AbstractServiceMessageMessage, new()
		{
			var message = CreateMessage<T>();
			var endpointFactory = CurrentMessage.Headers.ObjectBuilder.GetInstance<IEndpointFactory>();

			endpointFactory.GetEndpoint(uri).Send(message, c => c.SetSourceAddress(Bus.Endpoint.Uri));
		}


		private void UpdateQuorum<T>()
			where T : AbstractServiceMessageMessage, new()
		{
			var message = CreateMessage<T>();
			var endpointFactory = CurrentMessage.Headers.ObjectBuilder.GetInstance<IEndpointFactory>();

			Quorum.Each(x =>
				{
					if (x.ControlUri != ControlUri)
						endpointFactory.GetEndpoint(x.ControlUri).Send(message, c => c.SetSourceAddress(Bus.Endpoint.Uri));
				});
		}

		private void InitiateProposal()
		{
			UpdateQuorum<ProposeServiceMessage>();
		}

		private T CreateMessage<T>()
			where T : AbstractServiceMessageMessage, new()
		{
			return new T
				{
					BallotId = BallotId,
					CorrelationId = CorrelationId,
					ControlUri = ControlUri,
					DataUri = DataUri,
					Quorum = Quorum.Select(x => x.ControlUri).ToList(),
				};
		}
	}
}