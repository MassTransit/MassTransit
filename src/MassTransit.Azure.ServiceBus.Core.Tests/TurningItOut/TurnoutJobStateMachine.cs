// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Azure.ServiceBus.Core.Tests.TurningItOut
{
    using Automatonymous;
    using Turnout.Contracts;


    public sealed class TurnoutJobStateMachine :
        MassTransitStateMachine<TurnoutJobState>
    {
        public TurnoutJobStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => Started, x =>
            {
                x.CorrelateById(m => m.ConversationId ?? m.Message.JobId);
            });

            Event(() => Completed, x =>
            {
                x.CorrelateById(m => m.ConversationId ?? m.Message.JobId);
            });

            Event(() => Faulted, x =>
            {
                x.CorrelateById(m => m.ConversationId ?? m.Message.JobId);
            });


            Initially(
                When(Started)
                    .Then(OnStarted)
                    .TransitionTo(Active));

            During(Active,
                When(Completed)
                    .Then(OnCompleted)
//                    .Publish(context => new TurnoutJobCompleted
//                    {
//                        EndTime = context.Instance.EndTime,
//                        Id = context.Instance.Id,
//                        JobId = context.Instance.CorrelationId,
//                        StartTime = context.Instance.StartTime
//                    })
                    .TransitionTo(Complete),
                When(Faulted)
                    .TransitionTo(Failed));

            CompositeEvent(() => Finished, x => x.FinishedEvents, CompositeEventOptions.IncludeInitial, Started, Completed);

            DuringAny(
                When(Finished)
                    .Publish(context => new TurnoutJobCompleted
                    {
                        EndTime = context.Instance.EndTime,
                        Id = context.Instance.Id,
                        JobId = context.Instance.CorrelationId,
                        StartTime = context.Instance.StartTime
                    }));
        }

        public State Active { get; private set; }
        public State Complete { get; private set; }
        public State Failed { get; private set; }

        public Event<JobStarted> Started { get; private set; }
        public Event<JobCompleted> Completed { get; private set; }
        public Event<JobFaulted> Faulted { get; private set; }

        public Event Finished { get; private set; }

        static void OnStarted(BehaviorContext<TurnoutJobState, JobStarted> context)
        {
            context.Instance.Id = context.Data.GetArguments<LongTimeJob>().Id;
            context.Instance.ServerAddress = context.Data.ManagementAddress;
            context.Instance.StartTime = context.Data.Timestamp;
        }

        static void OnCompleted(BehaviorContext<TurnoutJobState, JobCompleted> context)
        {
            context.Instance.EndTime = context.Data.Timestamp;
        }
    }
}