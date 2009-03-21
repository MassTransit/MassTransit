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

	public class SubscriptionClientSaga :
		SagaStateMachine<SubscriptionClientSaga>,
		ISaga,
		InitiatedBy<CacheUpdateRequest>,
		Orchestrates<CancelSubscriptionUpdates>
	{
		private static readonly AddSubscriptionClientMapper _addMapper = new AddSubscriptionClientMapper();
		private static readonly RemoveSubscriptionClientMapper _removeMapper = new RemoveSubscriptionClientMapper();

		static SubscriptionClientSaga()
		{
			Define(() =>
				{
					Initially(
						When(ClientAdded)
							.Then((saga, message) =>
								{
									saga.EndpointUri = message.RequestingUri;

									saga.Bus.Publish(_addMapper.Transform(message));
								}).TransitionTo(Active));

					During(Active,
						When(ClientRemoved)
							.Then((saga, message) =>
								{
									// republish as a removal for the service to handle
									saga.Bus.Publish(_removeMapper.Transform(message));
								}).Complete());
				});
		}

		public SubscriptionClientSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected SubscriptionClientSaga()
		{
		}

		public static State Initial { get; set; }
		public static State Active { get; set; }
		public static State Completed { get; set; }

		public static Event<CacheUpdateRequest> ClientAdded { get; set; }
		public static Event<CancelSubscriptionUpdates> ClientRemoved { get; set; }
		public Uri EndpointUri { get; set; }


		public void Consume(CacheUpdateRequest message)
		{
			RaiseEvent(ClientAdded, message);
		}

		public Guid CorrelationId { get; set; }
		public IServiceBus Bus { get; set; }

		public void Consume(CancelSubscriptionUpdates message)
		{
			RaiseEvent(ClientRemoved, message);
		}
	}
}