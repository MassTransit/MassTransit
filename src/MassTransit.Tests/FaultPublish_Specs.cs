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
namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Testing;
    using MassTransit.Testing.MessageObservers;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class A_faulting_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_publish_a_single_fault_when_retried()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            _consumer.Faults.Select().Count().ShouldBe(1);
        }

        PingConsumer _consumer;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new PingConsumer(TimeSpan.FromSeconds(5));

            configurator.UseRetry(r => r.Immediate(5));
            _consumer.Configure(configurator);
        }


        class PingConsumer :
            MultiTestConsumer
        {
            readonly ReceivedMessageList<PingMessage> _messages;
            ReceivedMessageList<Fault<PingMessage>> _faults;

            public PingConsumer(TimeSpan timeout)
                : base(timeout)
            {
                _messages = Fault<PingMessage>();
                _faults = Consume<Fault<PingMessage>>();
            }

            public IReceivedMessageList<PingMessage> Messages => _messages;
            public IReceivedMessageList<Fault<PingMessage>> Faults => _faults;
        }
    }
}