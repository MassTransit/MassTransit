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
namespace MassTransit.Grid.Paxos
{
	using System;
	using Magnum.StateMachine;
	using Saga;

	public class Acceptor<T> :
		SagaStateMachine<Acceptor<T>>,
		ISaga
	{
		static Acceptor()
		{
			Define(() =>
				{
					Initially(
						When(Prepare)
							.Then((saga, message) => saga.AcknowledgePrepare(message))
							.TransitionTo(Prepared),
						When(Accept)
							.Then((saga, message) => saga.AcceptValue(message))
							.TransitionTo(SteadyState));

					During(Prepared,
					       When(Prepare)
					       	.Then(AcceptPrepareIfValid),
					       When(Accept)
					       	.Then(AcceptValueIfAllowed)
					       	.TransitionTo(SteadyState));

					During(SteadyState,
					       When(Prepare)
					       	.Then(AcceptPrepareIfValid),
					       When(Accept)
					       	.Then(AcceptValueIfAllowed));
				});
		}

		public Acceptor(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected Acceptor()
		{
		}

		public static State Initial { get; set; }
		public static State Prepared { get; set; }
		public static State SteadyState { get; set; }
		public static State Completed { get; set; }

		public static Event<Prepare<T>> Prepare { get; set; }
		public static Event<Accept<T>> Accept { get; set; }

		public virtual Guid LeaderId { get; set; }
		public virtual long BallotId { get; set; }
		public virtual long ValueBallotId { get; set; }
		public virtual T Value { get; set; }

		public Guid CorrelationId { get; set; }

		public IServiceBus Bus { get; set; }

		private static void AcceptValueIfAllowed(Acceptor<T> saga, Accept<T> message)
		{
			if (saga.IsMessageFromAValidSource(message))
			{
				saga.AcceptValue(message);
			}
			else
				throw new InvalidOperationException("An accept was received from an unknown leader during prepare");
		}

		private static void AcceptPrepareIfValid(Acceptor<T> saga, PaxosMessageBase message)
		{
			if (saga.IsMessageFromAValidSource(message))
			{
				saga.AcknowledgePrepare(message);
			}
			else
			{
				saga.RejectPrepare(message);
			}
		}

		private bool IsMessageFromAValidSource(PaxosMessageBase message)
		{
			return message.BallotId > BallotId || (message.BallotId >= BallotId && message.LeaderId == LeaderId);
		}

		private void AcknowledgePrepare(PaxosMessageBase message)
		{
			BallotId = message.BallotId;
			LeaderId = message.LeaderId;

			CurrentMessage.Respond(new Promise<T>
				{
					CorrelationId = CorrelationId,
					BallotId = BallotId,
					LeaderId = LeaderId,
					Value = Value,
					ValueBallotId = ValueBallotId,
				});
		}

		private void RejectPrepare(PaxosMessageBase message)
		{
			CurrentMessage.Respond(new PrepareRejected<T>
				{
					CorrelationId = CorrelationId,
					BallotId = BallotId,
					LeaderId = LeaderId,
					Value = Value,
					ValueBallotId = ValueBallotId,
				});
		}

		private void AcceptValue(Accept<T> message)
		{
			BallotId = message.BallotId;
			ValueBallotId = message.BallotId;
			Value = message.Value;

			Bus.Publish(new Accepted<T>
				{
					BallotId = BallotId,
					CorrelationId = CorrelationId,
					IsFinal = false,
					Value = Value,
					ValueBallotId = ValueBallotId,
				});
		}
	}
}