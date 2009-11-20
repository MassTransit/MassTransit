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
namespace MassTransit.ServiceBus
{
	//old stuff
	using System;
	using Magnum.StateMachine;
	using Messages;
	using Saga;
	using Subscriptions.Messages;
	using OldAddSubscription = Subscriptions.Messages.AddSubscription;
	using OldRemoveSubscription = Subscriptions.Messages.RemoveSubscription;
	using OldCacheUpdateRequest = Subscriptions.Messages.CacheUpdateRequest;
	using OldCacheUpdateResponse = Subscriptions.Messages.CacheUpdateResponse;
	using OldCancelSubscriptionUpdates = Subscriptions.Messages.CancelSubscriptionUpdates;


	public class LegacySubscriptionClientSaga :
		SagaStateMachine<LegacySubscriptionClientSaga>,
		ISaga
	{
		static LegacySubscriptionClientSaga()
		{
			Define(() =>
				{
					Correlate(OldCacheUpdateRequested).By((saga, message) => saga.DataUri == message.RequestingUri);
					Correlate(OldSubscriptionAdded).By((saga, message) => saga.DataUri == message.Subscription.EndpointUri);
					Correlate(OldSubscriptionRemoved).By((saga, message) => saga.DataUri == message.Subscription.EndpointUri);

					Initially(
						When(OldCacheUpdateRequested)
							.Then(InitialAction)
							.TransitionTo(Active),
						When(OldSubscriptionAdded)
							.Then(InitialAdd)
                            .TransitionTo(Active),
						When(OldSubscriptionRemoved)
							.Then(InitialRemoved)
                            .TransitionTo(Active));

					During(Active,
						When(OldCancelSubscriptionUpdates)
							.Then((saga, message) => saga.NotifyLegacySubscriptionClientRemoved())
							.Complete());
				});
		}

		public LegacySubscriptionClientSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected LegacySubscriptionClientSaga()
		{
		}


		public static State Initial { get; set; }
		public static State Active { get; set; }
		public static State Completed { get; set; }

		public static Event<OldAddSubscription> OldSubscriptionAdded { get; set; }
		public static Event<OldRemoveSubscription> OldSubscriptionRemoved { get; set; }
		public static Event<OldCacheUpdateRequest> OldCacheUpdateRequested { get; set; }
		public static Event<OldCancelSubscriptionUpdates> OldCancelSubscriptionUpdates { get; set; }


		public virtual Uri ControlUri { get; set; }
		public virtual Uri DataUri { get; set; }

		public Guid CorrelationId { get; set; }
		public IServiceBus Bus { get; set; }

		private void NotifyLegacySubscriptionClientRemoved()
		{
			var message = new LegacySubscriptionClientRemoved
				{
					CorrelationId = CorrelationId,
					ControlUri = ControlUri,
					DataUri = DataUri,
				};

			Bus.Publish(message);
		}

		private void NotifyLegacySubscriptionClientAdded()
		{
			var message = new LegacySubscriptionClientAdded
				{
					ClientId = CorrelationId,
					ControlUri = ControlUri,
					DataUri = DataUri,
				};

			Bus.Publish(message);
		}

		private static void InitialAction(LegacySubscriptionClientSaga saga, OldCacheUpdateRequest message)
		{
			saga.ControlUri = saga.Bus.Endpoint.Uri;
			saga.DataUri = message.RequestingUri;

			saga.NotifyLegacySubscriptionClientAdded();
		}

		private static void InitialAdd(LegacySubscriptionClientSaga saga, OldAddSubscription message)
		{
			saga.ControlUri = saga.Bus.Endpoint.Uri;
			saga.DataUri = message.Subscription.EndpointUri; //TODO: is this right?

			saga.NotifyLegacySubscriptionClientAdded();
		}

		private static void InitialRemoved(LegacySubscriptionClientSaga saga, OldRemoveSubscription message)
		{
			saga.ControlUri = saga.Bus.Endpoint.Uri;
			saga.DataUri = message.Subscription.EndpointUri; //TODO: is this right?

			saga.NotifyLegacySubscriptionClientAdded();
		}
	}
}