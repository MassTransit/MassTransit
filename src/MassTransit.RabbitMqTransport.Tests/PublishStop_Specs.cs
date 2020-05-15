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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class PublishStop_Specs
    {
        [Test, Explicit]
        public async Task Should_start_and_stop_sync()
        {
            var queueUri = new Uri($"rabbitmq://localhost/test/input_queue2");

            var rabbitMqHostSettings = queueUri.GetHostSettings();
            var receiveSettings = queueUri.GetReceiveSettings();

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(rabbitMqHostSettings);
                sbc.ReceiveEndpoint(receiveSettings.QueueName, ep =>
                {
                });
            });

            bus.Start();
            await bus.Publish(new DummyMessage {ID = 1}).ConfigureAwait(false);
            bus.Stop();
        }

        [Test, Explicit]
        public async Task Should_start_and_stop_async()
        {
            var queueUri = new Uri($"rabbitmq://localhost/test/input_queue2");

            var rabbitMqHostSettings = queueUri.GetHostSettings();
            var receiveSettings = queueUri.GetReceiveSettings();

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(rabbitMqHostSettings);
                sbc.ReceiveEndpoint(receiveSettings.QueueName, ep =>
                {
                });
            });

            await bus.StartAsync();
            await bus.Publish(new DummyMessage {ID = 1}).ConfigureAwait(false);
            await bus.StopAsync();
        }


        class DummyMessage
        {
            public int ID { get; set; }
        }
    }
}
