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
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using Util;


    [TestFixture]
    public class ContentFilter_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_get_one_message()
        {
            await InputQueueSendEndpoint.Send<TestMessage>(new {Key = "DENY"});
            await InputQueueSendEndpoint.Send<TestMessage>(new {Key = "ACCEPT"});

            await _accepted;
            await _denied;

            await Task.Delay(50);

            _consumer.Count.ShouldBe(1);
        }

        MyConsumer _consumer;
        Task<ConsumeContext<TestMessage>> _accepted;
        Task<ConsumeContext<TestMessage>> _denied;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _consumer = new MyConsumer();

            configurator.Consumer(() => _consumer, x =>
            {
                x.Message<TestMessage>(v => v.UseContextFilter(async context => context.Message.Key == "ACCEPT"));
            });

            _accepted = Handled<TestMessage>(configurator, x => x.Message.Key == "ACCEPT");
            _denied = Handled<TestMessage>(configurator, x => x.Message.Key == "DENY");
        }


        class MyConsumer :
            IConsumer<TestMessage>
        {
            int _count;

            public int Count => _count;

            public Task Consume(ConsumeContext<TestMessage> context)
            {
                Interlocked.Increment(ref _count);

                return TaskUtil.Completed;
            }
        }


        public interface TestMessage
        {
            string Key { get; }
        }
    }
}