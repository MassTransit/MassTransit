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
using NUnit.Framework;


namespace MassTransit.RabbitMqTransport.Tests
{
    using RabbitMQ.Client;


    [TestFixture]
    public class When_a_dead_letter_queue_is_specified :
        RabbitMqTestFixture
    {
        const string QueueName = "input-with-timeout";
        const string DeadLetterQueueName = "input-with-timeout-dlx";

        [Test]
        public void Should_create_and_bind_the_exchange_and_properties()
        {

        }

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            configurator.ReceiveEndpoint(QueueName, x =>
            {
                x.BindDeadLetterQueue(DeadLetterQueueName);
            });
        }

        protected override void OnCleanupVirtualHost(IModel model)
        {
            model.ExchangeDelete(QueueName);
            model.QueueDelete(QueueName);

            model.ExchangeDelete(DeadLetterQueueName);
            model.QueueDelete(DeadLetterQueueName);
        }
    }
}
