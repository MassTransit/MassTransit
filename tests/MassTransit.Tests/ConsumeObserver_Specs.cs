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
    namespace MyNamespace
    {
        using System.Linq;
        using System.Threading.Tasks;
        using MassTransit.Testing;
        using MassTransit.Testing.Observers;
        using NUnit.Framework;
        using Shouldly;
        using TestFramework;
        using TestFramework.Messages;


        [TestFixture, Category("Unit")]
        public class Observing_consumer_messages :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_trigger_the_consume_message_observer()
            {
                var context = await _pingObserver.PostConsumed;
            }

            [Test]
            public void Should_trigger_the_consume_observer()
            {
                IReceivedMessage<PingMessage> context = _observer.Messages.Select<PingMessage>().First();

                context.ShouldNotBeNull();
            }

            TestConsumeMessageObserver<PingMessage> _pingObserver;
            TestConsumeObserver _observer;

            [OneTimeSetUp]
            public async Task SetupObservers()
            {
                _pingObserver = GetConsumeObserver<PingMessage>();
                Bus.ConnectConsumeMessageObserver(_pingObserver);

                _observer = GetConsumeObserver();
                Bus.ConnectConsumeObserver(_observer);

                await Bus.Publish(new PingMessage());
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                Handled<PingMessage>(configurator);
            }
        }
    }
}