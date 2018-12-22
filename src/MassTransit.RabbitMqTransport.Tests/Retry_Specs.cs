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
    using System;
    using System.Threading.Tasks;
    using TestFramework.Messages;
    using NUnit.Framework;
    using GreenPipes;
    using GreenPipes.Introspection;
    using TestFramework;


    [TestFixture]
    public class When_specifying_retry_limit :
        RabbitMqTestFixture
    {
        readonly int _limit;
        int _attempts;

        public When_specifying_retry_limit()
        {
            _limit = 2;
            _attempts = 0;
        }

        [Test]
        public async Task Should_stop_after_limit_exceeded()
        {
            ProbeResult result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());

            await Bus.Publish(new PingMessage());
            await Task.Delay(TimeSpan.FromSeconds(20));
            Assert.LessOrEqual(_attempts, _limit + 1);
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            var sec2 = TimeSpan.FromSeconds(2);
            configurator.UseRetry(x => x.Exponential(_limit, sec2, sec2, sec2));

            configurator.Consumer(() => new Consumer(ref _attempts));
        }


        class Consumer : IConsumer<PingMessage>
        {
            public Consumer(ref int attempts)
            {
                ++attempts;
            }

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                throw new IntentionalTestException();
            }
        }
    }
}