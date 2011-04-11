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
namespace MassTransit.Tests.Load.Sagas
{
	using System;
	using Magnum.StateMachine;
	using MassTransit.Saga;
	using Messages;

	public class FirstSaga :
		SagaStateMachine<FirstSaga>,
		ISaga
	{
		static FirstSaga()
		{
			Define(Saga);
		}

		public FirstSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected FirstSaga()
		{
		}

		public static State Initial { get; set; }
		public static State Pending { get; set; }
		public static State Completed { get; set; }

		public static Event<FirstCommand> CommandReceived { get; set; }
		public static Event<FirstPending> PendingReceived { get; set; }

		public virtual Guid CorrelationId { get; set; }
		public virtual IServiceBus Bus { get; set; }

		private static void Saga()
		{
			Correlate(PendingReceived)
				.By((saga, message) => saga.CorrelationId == message.CorrelationId);

			Initially(
				When(CommandReceived)
					.RespondWith((saga, message) => new FirstPending(saga.CorrelationId))
					.TransitionTo(Pending));

			During(Pending,
				When(PendingReceived)
					.RespondWith((saga, message) => new FirstResponse(saga.CorrelationId))
					.Complete());
		}
	}
}