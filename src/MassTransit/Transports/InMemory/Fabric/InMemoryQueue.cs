// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.InMemory.Fabric
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    public class InMemoryQueue :
        IInMemoryQueue
    {
        readonly CancellationTokenSource _cancellationToken;
        readonly TaskCompletionSource<IInMemoryQueueConsumer> _consumer;
        readonly string _name;
        readonly LimitedConcurrencyLevelTaskScheduler _scheduler;
        int _queueDepth;

        public InMemoryQueue(string name, int concurrencyLevel)
        {
            _name = name;
            _scheduler = new LimitedConcurrencyLevelTaskScheduler(concurrencyLevel);
            _cancellationToken = new CancellationTokenSource();

            _consumer = new TaskCompletionSource<IInMemoryQueueConsumer>();
            _cancellationToken.Token.Register(() => _consumer.TrySetCanceled());
        }

        public void ConnectConsumer(IInMemoryQueueConsumer consumer)
        {
            try
            {
                _consumer.SetResult(consumer);
            }
            catch (Exception exception)
            {
                throw new ConfigurationException($"Only a single consumer can be connected to a queue: {_name}", exception);
            }
        }

        public Task Deliver(DeliveryContext<InMemoryTransportMessage> context)
        {
            if (context.WasAlreadyDelivered(this))
                return Task.FromResult(false);

            Interlocked.Increment(ref _queueDepth);

            Task.Factory.StartNew(() => DispatchMessage(context.Package), _cancellationToken.Token, TaskCreationOptions.None, _scheduler);

            return Task.FromResult(true);
        }

        async Task DispatchMessage(InMemoryTransportMessage message)
        {
            var consumer = await _consumer.Task;

            if (_cancellationToken.IsCancellationRequested)
                return;

            try
            {
                await consumer.Consume(message, _cancellationToken.Token);
            }
            catch
            {
            }
            finally
            {
                Interlocked.Decrement(ref _queueDepth);
            }
        }

        public override string ToString()
        {
            return $"Queue({_name})";
        }
    }
}