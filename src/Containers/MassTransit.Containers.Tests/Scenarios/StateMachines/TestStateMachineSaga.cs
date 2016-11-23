// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests.Scenarios.StateMachines
{
    using Automatonymous;


    public class TestStateMachineSaga :
        MassTransitStateMachine<TestInstance>
    {
        public TestStateMachineSaga()
        {
            Event(() => Updated, x => x.CorrelateBy(p => p.Key, m => m.Message.TestKey));

            Initially(
                When(Started)
                    .Then(context => context.Instance.Key = context.Data.TestKey)
                    .Activity(x => x.OfInstanceType<PublishTestStartedActivity>())
                    .TransitionTo(Active));

            During(Active,
                When(Updated)
                    .Publish(context => new TestUpdated {CorrelationId = context.Instance.CorrelationId, TestKey = context.Instance.Key})
                    .TransitionTo(Done)
                    .Finalize());

            SetCompletedWhenFinalized();
        }

        public State Active { get; private set; }
        public State Done { get; private set; }

        public Event<StartTest> Started { get; private set; }
        public Event<UpdateTest> Updated { get; private set; }
    }
}