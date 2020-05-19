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
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Util;


    [TestFixture]
    public class Creating_a_service_with_many_queues :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_not_exploded()
        {
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            for (var i = 0; i < 50; i++)
            {
                configurator.ReceiveEndpoint($"receiver_queue{i}", e =>
                {
                    e.Consumer<TestConsumer>();
                });
            }
        }


        class TestConsumer :
            IConsumer<TestMessage>
        {
            public Task Consume(ConsumeContext<TestMessage> context)
            {
                return TaskUtil.Completed;
            }
        }


        public interface TestMessage
        {
        }
    }
}
