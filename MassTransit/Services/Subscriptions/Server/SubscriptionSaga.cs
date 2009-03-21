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
namespace MassTransit.Services.Subscriptions.Server
{
	using System;
	using Magnum.StateMachine;
	using Saga;
	using Subscriptions.Messages;

	/// <summary>
	/// Manages the lifecycle of a subscription through the system
	/// </summary>
	public class SubscriptionSaga :
		SagaStateMachine<SubscriptionSaga>,
		ISaga,
		InitiatedBy<AddSubscription>,
		Orchestrates<RemoveSubscription>
	{
		private static readonly AddSubscriptionMapper _addMapper = new AddSubscriptionMapper();
		private static readonly RemoveSubscriptionMapper _removeMapper = new RemoveSubscriptionMapper();

		static SubscriptionSaga()
		{
			Define(() =>
				{
					Initially(
						When(SubscriptionAdded)
							.Then((saga, message) =>
								{
									// store the subscription information
									saga.SubscriptionInfo = message.Subscription;

									saga.Bus.Publish(_addMapper.Transform(message));
								}).TransitionTo(Active));

					During(Active,
						When(SubscriptionRemoved)
						.Then((saga,message)=>
							{
								// remove it
								saga.Bus.Publish(_removeMapper.Transform(message));
							})
							.Complete());
				});
		}

		public SubscriptionSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected SubscriptionSaga()
		{
		}


		public static State Initial { get; set; }
		public static State Active { get; set; }
		public static State Completed { get; set; }

		public static Event<AddSubscription> SubscriptionAdded { get; set; }
		public static Event<RemoveSubscription> SubscriptionRemoved { get; set; }

		public SubscriptionInformation SubscriptionInfo { get; set; }

		public void Consume(AddSubscription message)
		{
			RaiseEvent(SubscriptionAdded, message);
		}

		public IServiceBus Bus { get; set; }
		public Guid CorrelationId { get; set; }

		public void Consume(RemoveSubscription message)
		{
			RaiseEvent(SubscriptionRemoved, message);
		}
	}
}