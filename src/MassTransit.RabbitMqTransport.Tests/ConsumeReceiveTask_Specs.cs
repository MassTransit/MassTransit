// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class A_fault_publishing_a_message :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_not_cause_the_message_to_nack()
        {
            await Bus.Publish(new MyMessage { Value = "Hello, World." });

            await _faulted;

            await Task.Delay(1000);
        }

        Task<ConsumeContext<Fault<MyMessage>>> _faulted;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<MyMessage>(async context =>
            {
                await Console.Out.WriteLineAsync($"Received: {context.Message.Value}");

                await context.Publish(new MyMessage2 {Value = context.Message.Value + "  2"});
                await context.Publish(new MyMessage2()); // an exception will be caused by this action
            });
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input-queue-fault", x =>
            {
                _faulted = Handled<Fault<MyMessage>>(x);
            });
        }


        public class MyMessage
        {
            public string Value { get; set; }
        }


        public class MyMessage2
        {
            string _value;

            public string Value
            {
                get => _value ?? throw new IntentionalTestException("Simulate a serialization failure");
                set => _value = value;
            }
        }
    }
}
