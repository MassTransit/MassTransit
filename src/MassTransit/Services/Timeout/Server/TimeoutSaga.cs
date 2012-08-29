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
namespace MassTransit.Services.Timeout.Server
{
    using System;
    using Magnum.StateMachine;
    using Messages;
    using Saga;

    public class TimeoutSaga :
        SagaStateMachine<TimeoutSaga>,
        ISaga
    {
        static TimeoutSaga()
        {
            Define(() =>
                {
                    Correlate(SchedulingTimeout)
                        .By((saga, message) => saga.TimeoutId == message.CorrelationId && saga.Tag == message.Tag)
                        .UseNewId();
                    Correlate(CancellingTimeout).By(
                        (saga, message) => saga.TimeoutId == message.CorrelationId && saga.Tag == message.Tag);
                    Correlate(CompletingTimeout).By(
                        (saga, message) => saga.TimeoutId == message.CorrelationId && saga.Tag == message.Tag);

                    Initially(
                        When(SchedulingTimeout)
                            .Then((saga, message) =>
                                {
                                    saga.TimeoutId = message.CorrelationId;
                                    saga.Tag = message.Tag;
                                    saga.TimeoutAt = message.TimeoutAt;

                                    saga.NotifyTimeoutScheduled();
                                }).TransitionTo(WaitingForTime));

                    During(WaitingForTime,
                        When(CancellingTimeout)
                            .Then((saga, message) => { saga.NotifyTimeoutCancelled(); })
                            .Complete(),
                        When(SchedulingTimeout)
                            .Then((saga, message) =>
                                {
                                    saga.TimeoutAt = message.TimeoutAt;

                                    saga.NotifyTimeoutRescheduled();
                                }),
                        When(CompletingTimeout)
                            .Complete()
                        );

                    RemoveWhen(x => x.CurrentState == Completed);
                });
        }

        public TimeoutSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        protected TimeoutSaga()
        {
        }


        public static State Initial { get; set; }
        public static State WaitingForTime { get; set; }
        public static State Completed { get; set; }


        public static Event<ScheduleTimeout> SchedulingTimeout { get; set; }
        public static Event<CancelTimeout> CancellingTimeout { get; set; }
        public static Event<TimeoutExpired> CompletingTimeout { get; set; }

        public Guid TimeoutId { get; set; }
        public int Tag { get; set; }
        public DateTime TimeoutAt { get; set; }

        public Guid CorrelationId { get; set; }

        public IServiceBus Bus { get; set; }

        public bool Equals(TimeoutSaga obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.TimeoutId.Equals(TimeoutId) && obj.Tag == Tag;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(TimeoutSaga))
                return false;
            return Equals((TimeoutSaga)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Tag*397) ^ TimeoutId.GetHashCode();
            }
        }

        void NotifyTimeoutScheduled()
        {
            Bus.Publish(new TimeoutScheduled
                {
                    CorrelationId = TimeoutId,
                    TimeoutAt = TimeoutAt,
                    Tag = Tag,
                });
        }

        void NotifyTimeoutRescheduled()
        {
            Bus.Publish(new TimeoutRescheduled
                {
                    CorrelationId = TimeoutId,
                    TimeoutAt = TimeoutAt,
                    Tag = Tag,
                });
        }

        void NotifyTimeoutCancelled()
        {
            Bus.Publish(new TimeoutCancelled
                {
                    CorrelationId = TimeoutId,
                    Tag = Tag,
                });
        }
    }
}