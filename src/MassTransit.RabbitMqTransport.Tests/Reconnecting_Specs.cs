// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;


    [TestFixture]
    public class Reconnecting_Specs :
        RabbitMqTestFixture
    {
        ReconnectConsumer _consumer;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            _consumer = new ReconnectConsumer(TestTimeout);

            _consumer.Configure(configurator);
        }


        class ReconnectConsumer :
            MultiTestConsumer
        {
            public ReconnectConsumer(TimeSpan timeout)
                : base(timeout)
            {
                Consume<ReconnectMessage>();
            }
        }


        public class ReconnectMessage
        {
            public string Value { get; set; }
        }


        [Test, Explicit, Category("SlowAF")]
        public async Task Should_fault_nicely()
        {
            await Bus.Publish(new ReconnectMessage {Value = "Before"});

            bool beforeFound = await Task.Run(() => _consumer.Received.Select<ReconnectMessage>(x => x.Context.Message.Value == "Before").Any());
            Assert.IsTrue(beforeFound);

            Console.WriteLine("Okay, restart RabbitMQ");

            for (int i = 0; i < 20; i++)
            {
                await Task.Delay(1000);

                Console.Write($"{i}. ");
            }

            Console.WriteLine("");
            Console.WriteLine("Resuming");

            await Bus.Publish(new ReconnectMessage { Value = "After" });

            bool afterFound = await Task.Run(() => _consumer.Received.Select<ReconnectMessage>(x => x.Context.Message.Value == "After").Any());
            Assert.IsTrue(afterFound);
        }

        [Test, Explicit, Category("SlowAF")]
        public async Task Should_not_lock_when_sending_during_unavailable()
        {
            Console.WriteLine("Okay, stop RabbitMQ");

            for (int i = 0; i < 15; i++)
            {
                await Task.Delay(1000);

                Console.Write($"{i}. ");
            }

            Console.WriteLine("Sending");

            Assert.That(async () => await Bus.Publish(new ReconnectMessage { Value = "Before" }), Throws.TypeOf<RabbitMqConnectionException>());

            Console.WriteLine("Start it back up");

            for (int i = 0; i < 15; i++)
            {
                await Task.Delay(1000);

                Console.Write($"{i}. ");
            }

            Console.WriteLine("Sending");

            await Bus.Publish(new ReconnectMessage { Value = "After" });

            Console.WriteLine("Sent");

            bool afterFound = await Task.Run(() => _consumer.Received.Select<ReconnectMessage>(x => x.Context.Message.Value == "After").Any());
            Assert.IsTrue(afterFound);
        }
    }
}