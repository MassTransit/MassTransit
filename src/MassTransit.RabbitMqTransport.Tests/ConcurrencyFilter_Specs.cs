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
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class Using_a_consumer_concurrency_limit :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_limit_the_consumer()
        {
            _complete = GetTask<bool>();

            for (var i = 0; i < _messageCount; i++)
            {
                Bus.Publish(new A());
                Bus.Publish(new B());
            }

            await _complete.Task;

            Assert.AreEqual(2, _consumer.MaxDeliveryCount);
        }

        Consumer _consumer;
        static int _messageCount = 100;
        static TaskCompletionSource<bool> _complete;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            _consumer = new Consumer();

            configurator.Instance(_consumer, x => x.UseConcurrentMessageLimit(2));
        }


        class Consumer :
            IConsumer<A>,
            IConsumer<B>
        {
            int _currentPendingDeliveryCount;
            long _deliveryCount;
            int _maxPendingDeliveryCount;

            public int MaxDeliveryCount => _maxPendingDeliveryCount;

            public Task Consume(ConsumeContext<A> context)
            {
                return OnConsume();
            }

            public Task Consume(ConsumeContext<B> context)
            {
                return OnConsume();
            }

            async Task OnConsume()
            {
                Interlocked.Increment(ref _deliveryCount);

                var current = Interlocked.Increment(ref _currentPendingDeliveryCount);
                while (current > _maxPendingDeliveryCount)
                    Interlocked.CompareExchange(ref _maxPendingDeliveryCount, current, _maxPendingDeliveryCount);

                await Task.Delay(100);

                Interlocked.Decrement(ref _currentPendingDeliveryCount);

                if (_deliveryCount == _messageCount)
                    _complete.TrySetResult(true);
            }
        }


        class A
        {
        }


        class B
        {
        }
    }
}
