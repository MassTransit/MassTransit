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

    public class SubscriptionClientSaga :
        SagaStateMachine<SubscriptionClientSaga>,
        ISaga
    {
        static SubscriptionClientSaga()
        {
            Define(() =>
                {
                    Correlate(ClientRemoved)
                        .By(
                            (saga, message) =>
                            saga.CorrelationId == message.CorrelationId && saga.CurrentState == Active);

                    Correlate(DuplicateClientAdded)
                        .By((saga, message) => saga.ControlUri == message.ControlUri &&
                                               saga.CorrelationId != message.ClientId &&
                                               saga.CurrentState == Active);

                    Initially(
                        When(ClientAdded)
                            .Then((saga, message) =>
                                {
                                    saga.ControlUri = message.ControlUri;
                                    saga.DataUri = message.DataUri;

                                    saga.NotifySubscriptionClientAdded();
                                })
                            .TransitionTo(Active));

                    During(Active,
                        When(ClientRemoved)
                            .Then((saga, message) => saga.NotifySubscriptionClientRemoved())
                            .Complete(),
                        When(DuplicateClientAdded)
                            .Then((saga, message) => saga.NotifyDuplicateSubscriptionClientRemoved())
                            .Complete());

                    RemoveWhen(x => x.CurrentState == Completed);
                });
        }

        [UsedImplicitly]
        public SubscriptionClientSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        [UsedImplicitly]
        protected SubscriptionClientSaga()
        {
        }

        public static State Initial { get; set; }
        public static State Active { get; set; }
        public static State Completed { get; set; }

        public static Event<AddSubscriptionClient> ClientAdded { get; set; }
        public static Event<RemoveSubscriptionClient> ClientRemoved { get; set; }
        public static Event<SubscriptionClientAdded> DuplicateClientAdded { get; set; }

        public Uri ControlUri { get; set; }
        public Uri DataUri { get; set; }

        public Guid CorrelationId { get; set; }

        public IServiceBus Bus { get; set; }

        void NotifySubscriptionClientAdded()
        {
            var message = new SubscriptionClientAdded
                {
                    ClientId = CorrelationId,
                    ControlUri = ControlUri,
                    DataUri = DataUri,
                };

            Bus.Publish(message);
        }

        void NotifySubscriptionClientRemoved()
        {
            var message = new SubscriptionClientRemoved
                {
                    CorrelationId = CorrelationId,
                    ControlUri = ControlUri,
                    DataUri = DataUri,
                };

            Bus.Publish(message);
        }

        void NotifyDuplicateSubscriptionClientRemoved()
        {
            var message = new DuplicateSubscriptionClientRemoved
                {
                    CorrelationId = CorrelationId,
                    ControlUri = ControlUri,
                    DataUri = DataUri,
                };

            Bus.Publish(message);
        }
    }
}