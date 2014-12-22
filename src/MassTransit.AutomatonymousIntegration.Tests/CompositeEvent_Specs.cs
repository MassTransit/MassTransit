// Copyright 2011 Chris Patterson, Dru Sellers
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
namespace MassTransit.AutomatonymousTests
{
    using System;
    using System.Diagnostics;
    using Automatonymous;
    using NUnit.Framework;
    using Saga;
    using SubscriptionConfigurators;

    public static class TestHelper
    {
        public static TimeSpan MessageTimeout
        {
            get
            {
                return Debugger.IsAttached
                           ? TimeSpan.FromMinutes(30)
                           : TimeSpan.FromSeconds(8);
            }
        }
    }

//    [TestFixture]
//    public class When_combining_events_into_a_single_event :
//        MassTransitTestFixture
//    {
//        [Test]
//        public void Should_have_called_combined_event()
//        {
//            Bus.InboundPipeline.Trace();
//
//            var responseReceived = new FutureMessage<CompleteMessage>();
//
//            Bus.SubscribeHandler<CompleteMessage>(responseReceived.Set);
//
//            var message = new StartMessage();
//
//            Bus.Publish(message);
//            Bus.Publish(new FirstMessage(message.CorrelationId));
//            Bus.Publish(new SecondMessage(message.CorrelationId));
//
//            Assert.IsTrue(responseReceived.IsAvailable(TestHelper.MessageTimeout));
//        }
//
//
//        InMemorySagaRepository<Instance> _repository;
//
//        protected override void ConfigureSubscriptions(SubscriptionBusServiceConfigurator configurator)
//        {
//            _machine = new TestStateMachine();
//            _repository = new InMemorySagaRepository<Instance>();
//
////            configurator.StateMachineSaga(_machine, _repository);
//        }
//
//        TestStateMachine _machine;
//
//
//        class Instance :
//            SagaStateMachineInstance
//        {
//            public Instance(Guid correlationId)
//            {
//                CorrelationId = correlationId;
//            }
//
//            protected Instance()
//            {
//            }
//
//            public CompositeEventStatus CompositeStatus { get; set; }
//            public State CurrentState { get; set; }
//
//            public Guid CorrelationId { get; private set; }
//            public IServiceBus Bus { get; set; }
//        }
//
//
//        class TestStateMachine :
//            AutomatonymousStateMachine<Instance>
//        {
//            public TestStateMachine()
//            {
//                InstanceState(x => x.CurrentState);
//                State(() => Waiting);
//
//                Event(() => Start);
//                Event(() => First);
//                Event(() => Second);
//                Event(() => Third, x => x.CompositeStatus, First, Second);
//
//                Initially(
//                    When(Start)
//                        .TransitionTo(Waiting));
//
//                During(Waiting,
//                    When(Third)
//                        .Publish(context => new CompleteMessage(context.Instance.CorrelationId))
//                        .Finalize());
//            }
//
//            public State Waiting { get; private set; }
//
//            public Event<StartMessage> Start { get; private set; }
//
//            public Event<FirstMessage> First { get; private set; }
//            public Event<SecondMessage> Second { get; private set; }
//            public Event Third { get; private set; }
//        }
//
//
//        class StartMessage :
//            CorrelatedBy<Guid>
//        {
//            public StartMessage()
//            {
//                CorrelationId = NewId.NextGuid();
//            }
//
//            public Guid CorrelationId { get; set; }
//        }
//
//
//        class FirstMessage :
//            CorrelatedBy<Guid>
//        {
//            public FirstMessage(Guid correlationId)
//            {
//                CorrelationId = correlationId;
//            }
//
//            public Guid CorrelationId { get; set; }
//        }
//
//
//        class SecondMessage :
//            CorrelatedBy<Guid>
//        {
//            public SecondMessage(Guid correlationId)
//            {
//                CorrelationId = correlationId;
//            }
//
//            public Guid CorrelationId { get; set; }
//        }
//
//
//        class CompleteMessage :
//            CorrelatedBy<Guid>
//        {
//            public CompleteMessage(Guid correlationId)
//            {
//                CorrelationId = correlationId;
//            }
//
//            public Guid CorrelationId { get; set; }
//        }
//    }
}