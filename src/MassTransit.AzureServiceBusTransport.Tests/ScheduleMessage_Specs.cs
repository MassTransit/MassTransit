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
namespace MassTransit.AzureServiceBusTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using AzureServiceBusTransport;
    using Configuration;
    using NUnit.Framework;


    [TestFixture]
    public class ScheduleMessage_Specs :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_get_both_messages()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            await _second;
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.ScheduleMessage(DateTime.Now, new SecondMessage());
            });

            _second = Handled<SecondMessage>(configurator);
        }


        public class FirstMessage
        {
        }


        public class SecondMessage
        {
        }
    }

    public class Scheduling_a_published_message :
    AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_get_both_messages()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            await _second;
        }

        protected override void ConfigureBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            base.ConfigureBusHost(configurator, host);

            configurator.UseServiceBusMessageScheduler();
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.SchedulePublish(TimeSpan.FromSeconds(1), new SecondMessage());
            });

            _second = Handled<SecondMessage>(configurator);
        }


        public class FirstMessage
        {
        }


        public class SecondMessage
        {
        }
    }

}