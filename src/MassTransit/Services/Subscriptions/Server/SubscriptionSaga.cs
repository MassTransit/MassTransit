// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Messages;
    using Saga;
    using Subscriptions.Messages;
    using Util;

    /// <summary>
    ///   Manages the lifecycle of a subscription through the system
    /// </summary>
    public class SubscriptionSaga :
        SagaStateMachine<SubscriptionSaga>,
        ISaga
    {
        static SubscriptionSaga()
        {
            Define(() =>
                {
                    Correlate(DuplicateClientRemoved)
                        .By(
                            (saga, message) =>
                            saga.SubscriptionInfo.ClientId == message.CorrelationId && saga.CurrentState == Active);

                    Correlate(ClientAdded)
                        .By((saga, message) => saga.SubscriptionInfo.EndpointUri == message.DataUri
                                               && saga.SubscriptionInfo.ClientId != message.ClientId
                                               && saga.CurrentState == Active);

                    Correlate(SubscriptionRemoved)
                        .By(
                            (saga, message) =>
                            saga.SubscriptionInfo.SubscriptionId == message.CorrelationId && saga.CurrentState == Active);

                    Correlate(DuplicateSubscriptionAdded)
                        .By((saga, message) => saga.SubscriptionInfo.MessageName == message.Subscription.MessageName
                                               && saga.SubscriptionInfo.EndpointUri == message.Subscription.EndpointUri
                                               && saga.SubscriptionInfo.ClientId != message.Subscription.ClientId
                                               && saga.CurrentState == Active);

                    Initially(
                        When(SubscriptionAdded)
                            .Then((saga, message) =>
                                {
                                    saga.SubscriptionInfo = message.Subscription;
                                    saga.NotifySubscriptionAdded();
                                })
                            .TransitionTo(Active));

                    During(Active,
                        When(SubscriptionRemoved)
                            .Then((saga, message) => saga.NotifySubscriptionRemoved(message.Subscription))
                            .Complete(),
                        When(DuplicateSubscriptionAdded)
                            .Then((saga, message) => saga.NotifySubscriptionRemoved(saga.SubscriptionInfo))
                            .Complete(),
                        When(DuplicateClientRemoved)
                            .Then((saga, message) => saga.NotifySubscriptionRemoved())
                            .Complete(),
                        When(ClientAdded)
                            .Then((saga, message) => saga.NotifySubscriptionRemoved())
                            .Complete()
                        );

                    RemoveWhen(x => x.CurrentState == Completed);
                });
        }

        [UsedImplicitly]
        public SubscriptionSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        [UsedImplicitly]
        protected SubscriptionSaga()
        {
        }


        public static State Initial { get; set; }
        public static State Active { get; set; }
        public static State Completed { get; set; }

        public static Event<AddSubscription> SubscriptionAdded { get; set; }
        public static Event<AddSubscription> DuplicateSubscriptionAdded { get; set; }
        public static Event<RemoveSubscription> SubscriptionRemoved { get; set; }

        public static Event<DuplicateSubscriptionClientRemoved> DuplicateClientRemoved { get; set; }
        public static Event<SubscriptionClientAdded> ClientAdded { get; set; }

        public SubscriptionInformation SubscriptionInfo { get; set; }

        public IServiceBus Bus { get; set; }

        public Guid CorrelationId { get; set; }

        void NotifySubscriptionAdded()
        {
            Bus.Publish(new SubscriptionAdded {Subscription = SubscriptionInfo});
        }

        void NotifySubscriptionRemoved()
        {
            NotifySubscriptionRemoved(SubscriptionInfo);
        }

        void NotifySubscriptionRemoved(SubscriptionInformation subscription)
        {
            Bus.Publish(new SubscriptionRemoved {Subscription = subscription});
        }
    }
}