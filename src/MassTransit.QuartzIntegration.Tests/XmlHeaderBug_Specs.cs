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
namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Internals.Extensions;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class XmlHeaderBug_Specs :
        QuartzInMemoryTestFixture
    {
        Consumer _consumer;

        [Test]
        public async Task Should_not_get_junk_headers()
        {
            await Bus.Publish<IMyMessage>(new {Description = "hi!"});

            await _consumer.Completed;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.UseXmlSerializer();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new Consumer();
            configurator.Instance(_consumer);
        }


        public interface IMyMessage
        {
            string Description { get; }
        }


        class Consumer :
            IConsumer<IMyMessage>
        {
            public Task Completed => _source.Task;

            readonly TaskCompletionSource<ConsumeContext<IMyMessage>> _source = TaskCompletionSourceFactory.New<ConsumeContext<IMyMessage>>();

            public async Task Consume(ConsumeContext<IMyMessage> context)
            {
                if (context.Headers.TryGetHeader("MT-Redelivery-Count", out var value))
                {
                    if (context.Headers.TryGetHeader("#text", out var val))
                    {
                        _source.TrySetException(new Exception("The bogus text header was present"));
                        return;
                    }

                    _source.TrySetResult(context);
                    return;
                }

                if (value == null)
                {
                    await context.Redeliver(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}
