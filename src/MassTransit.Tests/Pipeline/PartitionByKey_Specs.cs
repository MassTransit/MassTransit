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
namespace MassTransit.Tests.Pipeline
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using GreenPipes;
    using Util;


    [TestFixture]
    public class Partitioning_a_consumer_by_key :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_use_a_partitioner_for_consistency()
        {
            await Task.WhenAll(Enumerable.Range(0, Limit).Select(index => Bus.Publish(new PartitionedMessage {CorrelationId = NewId.NextGuid()})));

            var count = await _completed.Task;

            Assert.AreEqual(Limit, count);

            Console.WriteLine("Processed: {0}", count);

            //Console.WriteLine(Bus.GetProbeResult().ToJsonString());
        }

        TaskCompletionSource<int> _completed;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _completed = GetTask<int>();

            configurator.Consumer(() => new PartitionedConsumer(_completed), x =>
            {
                x.Message<PartitionedMessage>(m =>
                {
                    m.UsePartitioner(8, context => context.Message.CorrelationId);
                });
            });
        }

        const int Limit = 100;


        class PartitionedConsumer :
            IConsumer<PartitionedMessage>
        {
            static int _count;
            readonly TaskCompletionSource<int> _completed;

            public PartitionedConsumer(TaskCompletionSource<int> completed)
            {
                _completed = completed;
            }

            public Task Consume(ConsumeContext<PartitionedMessage> context)
            {
                if (Interlocked.Increment(ref _count) == Limit)
                    _completed.TrySetResult(Limit);

                return TaskUtil.Completed;
            }
        }


        class PartitionedMessage
        {
            public Guid CorrelationId { get; set; }
        }
    }
}