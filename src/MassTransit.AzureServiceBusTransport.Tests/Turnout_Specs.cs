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
namespace MassTransit.AzureServiceBusTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using AzureServiceBusTransport;
    using NUnit.Framework;
    using Turnout.Contracts;


    [TestFixture]
    public class Creating_a_turnout_job :
        AzureServiceBusTestFixture
    {
        [Test]
        [Category("Flakey")]
        public async Task Should_allow_scheduling_a_job()
        {
            var endpoint = await Bus.GetSendEndpoint(_commandEndpointAddress);

            await endpoint.Send(new ProcessFile
            {
                Filename = "log.txt",
                Size = 1
            });

            ConsumeContext<JobCompleted> context = await _completed;

            await endpoint.Send(new ProcessFile
            {
                Filename = "log.txt",
                Size = 2
            });

            context = await _completed2;
        }


        public class ProcessFile
        {
            public string Filename { get; set; }
            public int Size { get; set; }
        }


        Task<ConsumeContext<JobCompleted>> _completed;
        Uri _commandEndpointAddress;
        Task<ConsumeContext<JobCompleted>> _completed2;

        protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            configurator.UseServiceBusMessageScheduler();

            base.ConfigureServiceBusBusHost(configurator, host);

            configurator.TurnoutEndpoint<ProcessFile>(host, "process_queue", endpoint =>
            {
                endpoint.SuperviseInterval = TimeSpan.FromSeconds(1);
                endpoint.SetJobFactory(async context => await Task.Delay(TimeSpan.FromSeconds(context.Command.Size)).ConfigureAwait(false));

                _commandEndpointAddress = endpoint.InputAddress;
            });
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _completed = Handled<JobCompleted>(configurator, context => context.Message.GetArguments<ProcessFile>().Size == 1);
            _completed2 = Handled<JobCompleted>(configurator, context => context.Message.GetArguments<ProcessFile>().Size == 2);
        }
    }

    [TestFixture]
    public class Stopping_the_bus_before_the_job_is_done :
        AzureServiceBusTestFixture
    {
        [Test]
        [Category("Flakey")]
        public async Task Should_send_the_job_canceled()
        {
            var endpoint = await Bus.GetSendEndpoint(_commandEndpointAddress);

            await endpoint.Send(new ProcessFile
            {
                Filename = "log.txt",
                Size = 10
            });

            var context = await _started;

            Console.WriteLine("Started on address: {0}", context.Message.ManagementAddress);
        }


        public class ProcessFile
        {
            public string Filename { get; set; }
            public int Size { get; set; }
        }


        Task<ConsumeContext<JobStarted>> _started;
        Uri _commandEndpointAddress;

        protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            configurator.UseServiceBusMessageScheduler();

            base.ConfigureServiceBusBusHost(configurator, host);

            configurator.TurnoutEndpoint<ProcessFile>(host, "process_queue", endpoint =>
            {
                endpoint.SuperviseInterval = TimeSpan.FromSeconds(1);
                endpoint.SetJobFactory(async context =>
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(context.Command.Size), context.CancellationToken).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine("Task was canceled!");
                        throw;
                    }
                });

                _commandEndpointAddress = endpoint.InputAddress;
            });
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _started = Handled<JobStarted>(configurator);
        }
    }

    [TestFixture]
    public class Cancelling_a_job_using_the_management_address :
        AzureServiceBusTestFixture
    {
        [Test]
        [Category("Flakey")]
        public async Task Should_send_the_job_canceled()
        {
            var endpoint = await Bus.GetSendEndpoint(_commandEndpointAddress);

            await endpoint.Send(new ProcessFile
            {
                Filename = "log.txt",
                Size = 10
            });

            var context = await _started;

            Console.WriteLine("Started on address: {0}", context.Message.ManagementAddress);

            var managementEndpoint = await Bus.GetSendEndpoint(context.Message.ManagementAddress);

            await managementEndpoint.Send<CancelJob>(new
            {
                JobId = context.Message.JobId,
                Timestamp = DateTime.UtcNow,
                Reason = "Impatient!"
            });

            await _canceled;

            Console.WriteLine("Canceled, yea!");
        }


        public class ProcessFile
        {
            public string Filename { get; set; }
            public int Size { get; set; }
        }


        Task<ConsumeContext<JobStarted>> _started;
        Uri _commandEndpointAddress;
        Task<ConsumeContext<JobCanceled<ProcessFile>>> _canceled;

        protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            configurator.UseServiceBusMessageScheduler();

            base.ConfigureServiceBusBusHost(configurator, host);

            configurator.TurnoutEndpoint<ProcessFile>(host, "process_queue", endpoint =>
            {
                endpoint.SuperviseInterval = TimeSpan.FromSeconds(1);
                endpoint.SetJobFactory(async context =>
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(context.Command.Size), context.CancellationToken).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine("Task was canceled!");
                        throw;
                    }
                });

                _commandEndpointAddress = endpoint.InputAddress;
            });
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _started = Handled<JobStarted>(configurator);
            _canceled = Handled<JobCanceled<ProcessFile>>(configurator);
        }
    }
}
