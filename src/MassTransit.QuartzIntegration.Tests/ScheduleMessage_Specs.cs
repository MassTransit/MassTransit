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
namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class ScheduleMessage_Specs :
        QuartzInMemoryTestFixture
    {
        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.ScheduleSend(TimeSpan.FromSeconds(10), new SecondMessage());
            });

            _second = Handled<SecondMessage>(configurator);
        }


        public class FirstMessage
        {
        }


        public class SecondMessage
        {
        }


        [Test]
        public async Task Should_get_both_messages()
        {
            await Bus.ScheduleSend(InputQueueAddress, DateTime.Now, new FirstMessage());

            await _first;

            AdvanceTime(TimeSpan.FromSeconds(10));

            await _second;
        }
    }
}