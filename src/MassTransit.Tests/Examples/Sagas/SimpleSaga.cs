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
namespace MassTransit.Tests.Examples.Sagas
{
	using System;
	using Magnum.StateMachine;
	using MassTransit.Saga;
	using Messages;

	public class SimpleSaga :
		SagaStateMachine<SimpleSaga>,
		ISaga
	{
		static SimpleSaga()
		{
			Define(Saga);
		}

		private static void Saga()
		{
			Correlate(Approved)
				.By((saga, message) => saga.CustomerId == message.CustomerId);

			Initially(
				When(Started)
					.Then((saga, message) => saga.CustomerId = message.CustomerId)
					.TransitionTo(WaitingForApproval));

			During(WaitingForApproval,
				When(Approved)
					.Then((saga, message) => saga.Approver = message.ApprovedBy)
					.TransitionTo(WaitingForFinish));

			During(WaitingForFinish,
				When(Finished)
					.Complete());
		}

		public SimpleSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected SimpleSaga()
		{
		}

		public static State Initial { get; set; }
		public static State WaitingForApproval { get; set; }
		public static State WaitingForFinish { get; set; }
		public static State Completed { get; set; }

		public static Event<StartSimpleSaga> Started { get; set; }
		public static Event<ApproveSimpleCustomer> Approved { get; set; }
		public static Event<FinishSimpleSaga> Finished { get; set; }

		public virtual string Approver { get; set; }
		public virtual int CustomerId { get; set; }

		public virtual Guid CorrelationId { get; set; }
		public virtual IServiceBus Bus { get; set; }
	}
}