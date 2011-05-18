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
namespace MassTransit.Tests.Saga.StateMachine
{
	using System;
	using Magnum.StateMachine;
	using MassTransit.Saga;
	using NUnit.Framework;
	using TestFramework;
	using TextFixtures;

	[TestFixture]
	public class SagaFault_Specs :
		LoopbackTestFixture
	{
		InMemorySagaRepository<CreateCustomerSaga> _repository;

		protected override void EstablishContext()
		{
			base.EstablishContext();

			_repository = SetupSagaRepository<CreateCustomerSaga>();
		}

		[Test]
		public void The_saga_should_be_subscribable()
		{
			var unsubscribeAction = LocalBus.SubscribeSaga<CreateCustomerSaga>(_repository);


			unsubscribeAction();
		}
	}

	[Serializable]
	public class CreateCustomerSaga :
		SagaStateMachine<CreateCustomerSaga>,
		ISaga
	{
		static CreateCustomerSaga()
		{
			Define(() =>
				{
					Initially(
						When(Initialised)
							.Then((saga, message) => { saga.Bus.Publish(new CreateCustomerMessage()); })
							.TransitionTo(WaitingForCustomer),
						When(InitialisationFailed)
							.Then((saga, message) => saga.HandleError(message, "An error occurred while initiating the workflow"))
							.Complete());


					During(
						WaitingForCustomer,
						When(CustomerCreated).Where(message => message.CreateAgency)
							.Then((saga, message) => { saga.Bus.Publish(new CreateAgencyMessage()); })
							.TransitionTo(WaitingForAgency),
						When(CustomerCreated).Where(message => !message.CreateAgency)
							.Then((saga, message) => { saga.Bus.Publish(new UpdateAccountMessage()); })
							.TransitionTo(WaitingForAccount),
						When(CreateCustomerFailed)
							.Then((saga, message) => saga.HandleError(message, "An error occurred while creating the customer"))
							.Complete());


					During(
						WaitingForAgency,
						When(AgencyCreated)
							.Then((saga, message) => { saga.Bus.Publish(new UpdateAccountMessage()); })
							.TransitionTo(WaitingForAccount),
						When(CreateAgencyFailed)
							.Then((saga, message) => saga.HandleError(message, "An errror occurred while creating the agency"))
							.Complete());


					During(
						WaitingForAccount,
						When(UpdateAccount)
							.Then((saga, message) => { })
							.Complete());
				});
		}

		public CreateCustomerSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public static State Initial { get; set; }
		public static State WaitingForCustomer { get; set; }
		public static State WaitingForAgency { get; set; }
		public static State WaitingForAccount { get; set; }
		public static State Completed { get; set; }


		public static Event<InitiateCreateAccountMessage> Initialised { get; set; }
		public static Event<Fault<InitiateCreateAccountMessage, Guid>> InitialisationFailed { get; set; }
		public static Event<CustomerCreatedMessage> CustomerCreated { get; set; }
		public static Event<Fault<CreateCustomerMessage, Guid>> CreateCustomerFailed { get; set; }
		public static Event<AgencyCreatedMessage> AgencyCreated { get; set; }
		public static Event<Fault<CreateAgencyMessage, Guid>> CreateAgencyFailed { get; set; }
		public static Event<UpdateAccountMessage> UpdateAccount { get; set; }


		public Guid CorrelationId { get; set; }

		public IServiceBus Bus { get; set; }

		public void HandleError<T>(Fault<T> correlatedMessage, string errorSummary)
			where T : class
		{
			Console.WriteLine(correlatedMessage.OccurredAt);
			Console.WriteLine(correlatedMessage.CaughtException.StackTrace);
		}
	}

	public class CreateCustomerMessage :
		CorrelatedBy<Guid>
	{
		public Guid CorrelationId
		{
			get { throw new ATestException(); }
		}
	}

	public class AgencyCreatedMessage : CorrelatedBy<Guid>
	{
		public Guid CorrelationId
		{
			get { throw new ATestException(); }
		}
	}

	public class CreateAgencyMessage : CorrelatedBy<Guid>
	{
		public Guid CorrelationId
		{
			get { throw new ATestException(); }
		}
	}

	public class UpdateAccountMessage : CorrelatedBy<Guid>
	{
		public Guid CorrelationId
		{
			get { throw new ATestException(); }
		}
	}

	public class CustomerCreatedMessage : CorrelatedBy<Guid>
	{
		public bool CreateAgency { get; set; }

		public Guid CorrelationId
		{
			get { throw new ATestException(); }
		}
	}

	public class InitiateCreateAccountMessage : CorrelatedBy<Guid>
	{
		public Guid CorrelationId
		{
			get { throw new ATestException(); }
		}
	}
}