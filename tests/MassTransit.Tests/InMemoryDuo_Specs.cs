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
namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Shouldly;
    using Util;


    [TestFixture]
    public class Running_two_in_memory_transports
    {
        [Test]
        public async Task Should_keep_em_separated()
        {
            var internalHarness = new InMemoryTestHarness("internal");
            var externalHarness = new InMemoryTestHarness("external");

            ConsumerTestHarness<RelayConsumer> internalConsumer = internalHarness.Consumer(() => new RelayConsumer(externalHarness.Bus));
            ConsumerTestHarness<RelayConsumer> externalConsumer = externalHarness.Consumer(() => new RelayConsumer(internalHarness.Bus));

            var realConsumer = internalHarness.Consumer<RealConsumer>();

            await internalHarness.Start();
            try
            {
                await externalHarness.Start();
                try
                {
                    await externalHarness.Bus.Publish(new A());

                    realConsumer.Consumed.Select<A>().Any().ShouldBeTrue();
                }
                finally
                {
                    await externalHarness.Stop();
                }
            }
            finally
            {
                await internalHarness.Stop();
            }
        }


        class RelayConsumer :
            IConsumer<A>
        {
            readonly IPublishEndpoint _otherHost;

            public RelayConsumer(IPublishEndpoint otherHost)
            {
                _otherHost = otherHost;
            }

            public Task Consume(ConsumeContext<A> context)
            {
                if (GetVirtualHost(context.SourceAddress) != GetVirtualHost(context.ReceiveContext.InputAddress))
                    return TaskUtil.Completed;

                LogContext.Info?.Log("Forwarding message: {MessageId} from {SourceAddress}", context.MessageId, context.SourceAddress);

                IPipe<SendContext> contextPipe = context.CreateCopyContextPipe();

                return _otherHost.Publish(context.Message, contextPipe);
            }
        }


        static string GetVirtualHost(Uri address)
        {
            return address.AbsolutePath.Split('/').First(x => !string.IsNullOrWhiteSpace(x));

        }

        class RealConsumer :
            IConsumer<A>
        {
            public Task Consume(ConsumeContext<A> context)
            {
                return TaskUtil.Completed;
            }
        }


        public class A
        {
        }
    }
}
