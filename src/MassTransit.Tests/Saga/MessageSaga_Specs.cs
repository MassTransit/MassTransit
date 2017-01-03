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
namespace MassTransit.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class Configuring_a_message_in_a_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_all_the_stuff()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _message.Task;

            await _consumerOnly.Task;

            await _consumerMessage.Task;
        }

        TaskCompletionSource<PingMessage> _message;
        TaskCompletionSource<Tuple<MySaga, PingMessage>> _consumerMessage;
        TaskCompletionSource<MySaga> _consumerOnly;
        InMemorySagaRepository<MySaga> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _message = GetTask<PingMessage>();
            _consumerOnly = GetTask<MySaga>();
            _consumerMessage = GetTask<Tuple<MySaga, PingMessage>>();

            _repository = new InMemorySagaRepository<MySaga>();

            configurator.Saga(_repository, cfg =>
            {
                cfg.UseExecute(context => _consumerOnly.TrySetResult(context.Saga));
                cfg.Message<PingMessage>(m => m.UseExecute(context => _message.TrySetResult(context.Message)));
                cfg.SagaMessage<PingMessage>(m => m.UseExecute(context => _consumerMessage.TrySetResult(Tuple.Create(context.Saga, context.Message))));
            });
        }


        class MySaga :
            InitiatedBy<PingMessage>,
            ISaga
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return TaskUtil.Completed;
            }

            public Guid CorrelationId { get; set; }
        }
    }
}