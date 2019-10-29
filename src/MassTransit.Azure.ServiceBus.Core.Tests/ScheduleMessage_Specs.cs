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
namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;
    using Internals.Extensions;
    using MassTransit.Scheduling;
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

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.ScheduleSend(DateTime.Now, new SecondMessage());
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

    [TestFixture]
    public class Scheduling_a_message_in_the_future :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_get_both_messages()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            var timer = Stopwatch.StartNew();

            await _second;

            timer.Stop();

            Assert.That(timer.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(2)));
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.ScheduleSend(TimeSpan.FromSeconds(3), new SecondMessage());
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


    [TestFixture]
    public class Scheduling_a_message_using_quartz :
        AzureServiceBusTestFixture
    {
        TimeSpan _testOffset;

        public Scheduling_a_message_using_quartz()
        {
            _testOffset = TimeSpan.Zero;
            AzureServiceBusTestHarness.ConfigureMessageScheduler = false;
        }

        Uri QuartzAddress { get; set; }

        [Test]
        public async Task Should_get_the_message()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            await _second;
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                await context.ScheduleSend(DateTime.Now, new SecondMessage());
            });

            _second = Handled<SecondMessage>(configurator);
        }

        protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            QuartzAddress = configurator.UseInMemoryScheduler();
        }


        public class FirstMessage
        {
        }


        public class SecondMessage
        {
        }
    }


    [TestFixture]
    public class Scheduling_a_message_using_quartz_and_cancelling_it :
        AzureServiceBusTestFixture
    {
        TimeSpan _testOffset;

        public Scheduling_a_message_using_quartz_and_cancelling_it()
        {
            _testOffset = TimeSpan.Zero;
            AzureServiceBusTestHarness.ConfigureMessageScheduler = false;
        }

        Uri QuartzAddress { get; set; }

        [Test]
        public async Task Should_not_get_the_message()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            Assert.That(async () => await _second.OrTimeout(5000), Throws.TypeOf<TimeoutException>());
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _first = Handler<FirstMessage>(configurator, async context =>
            {
                ScheduledMessage<SecondMessage> scheduledMessage = await context.ScheduleSend(DateTime.Now + TimeSpan.FromSeconds(5), new SecondMessage());

                await Task.Delay(1000);

                await context.CancelScheduledSend(scheduledMessage);
            });

            _second = Handled<SecondMessage>(configurator);
        }

        protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            QuartzAddress = configurator.UseInMemoryScheduler();
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

        protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            base.ConfigureServiceBusBusHost(configurator, host);

            configurator.UseServiceBusMessageScheduler();
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
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


    public class Cancelling_a_scheduled_message :
        AzureServiceBusTestFixture
    {
        [Test, Explicit]
        public async Task Should_result_in_no_message_received()
        {
            await InputQueueSendEndpoint.Send(new FirstMessage());

            await _first;

            Assert.That(async () => await _second.OrTimeout(TimeSpan.FromSeconds(20)), Throws.TypeOf<OperationCanceledException>());
        }

        protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            base.ConfigureServiceBusBusHost(configurator, host);

            configurator.UseServiceBusMessageScheduler();
        }

        Task<ConsumeContext<SecondMessage>> _second;
        Task<ConsumeContext<FirstMessage>> _first;
        Guid _testId;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _testId = NewId.NextGuid();

            _first = Handler<FirstMessage>(configurator, async context =>
            {
                var scheduledMessage = await context.ScheduleSend(TimeSpan.FromSeconds(15), new SecondMessage() {Id = _testId});

                await Task.Delay(1000);

                await context.CancelScheduledSend(scheduledMessage);
            });

            _second = Handled<SecondMessage>(configurator, context => context.Message.Id == _testId);
        }


        public class FirstMessage
        {
        }


        public class SecondMessage
        {
            public Guid Id { get; set; }
        }
    }
}
