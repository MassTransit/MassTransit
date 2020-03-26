// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    public class HangfirePublish_Specs :
        HangfireInMemoryTestFixture
    {
        class SomeMessageConsumer :
            IConsumer<SomeMessage>
        {
            public Task Consume(ConsumeContext<SomeMessage> context)
            {
                return Console.Out.WriteLineAsync(context.Message.GetType().Name + " - " + context.Message.SendDate + " - " + context.Message.Source);
            }
        }


        class SomeMessage
        {
            public DateTime SendDate { get; set; }
            public string Source { get; set; }
        }


        [Test]
        public async Task Should_receive_the_message()
        {
            var handled = SubscribeHandler<SomeMessage>();

            Bus.ConnectConsumer(() => new SomeMessageConsumer());

            await Bus.ScheduleSend(Bus.Address, DateTime.Now, new SomeMessage
            {
                SendDate = DateTime.Now,
                Source = "Schedule"
            });

            await handled;
        }
    }
}
