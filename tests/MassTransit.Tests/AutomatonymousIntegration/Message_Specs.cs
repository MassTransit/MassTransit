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
namespace MassTransit.Tests.AutomatonymousIntegration
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Configuring_a_message_in_a_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_all_the_stuff()
        {
            await InputQueueSendEndpoint.Send(new Start());

            await _message.Task;

            await _consumerOnly.Task;

            await _consumerMessage.Task;
        }

        TaskCompletionSource<Start> _message;
        TaskCompletionSource<Tuple<Instance, Start>> _consumerMessage;
        TaskCompletionSource<Instance> _consumerOnly;
        InMemorySagaRepository<Instance> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _message = GetTask<Start>();
            _consumerOnly = GetTask<Instance>();
            _consumerMessage = GetTask<Tuple<Instance, Start>>();

            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(new TestStateMachine(), _repository, cfg =>
            {
                cfg.UseExecute(context => _consumerOnly.TrySetResult(context.Saga));
                cfg.Message<Start>(m => m.UseExecute(context => _message.TrySetResult(context.Message)));
                cfg.SagaMessage<Start>(m => m.UseExecute(context => _consumerMessage.TrySetResult(Tuple.Create(context.Saga, context.Message))));
            });
        }


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(Started)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Start()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Guid CorrelationId { get; private set; }
        }
    }
}