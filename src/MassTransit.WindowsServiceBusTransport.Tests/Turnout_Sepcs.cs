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
namespace MassTransit.WindowsServiceBusTransport.Tests
{
    namespace TestingTurnout
    {
        using System;
        using System.Threading.Tasks;
        using WindowsServiceBusTransport.Configuration;
        using NUnit.Framework;
        using Turnout.Contracts;


        [TestFixture]
        public class Creating_a_turnout_job :
            ServiceBusTestFixture
        {
            [Test]
            public async Task Should_allow_scheduling_a_job()
            {
                await Bus.Publish(new ProcessFile
                {
                    Filename = "log.txt",
                    Size = 10
                });

                var context = await _completed;
            }

            IServiceBusBusFactoryConfigurator _busFactoryConfigurator;
            Task<ConsumeContext<JobCompleted>> _completed;

            protected override void ConfigureBus(IServiceBusBusFactoryConfigurator configurator)
            {
                base.ConfigureBus(configurator);

                _busFactoryConfigurator = configurator;
            }

            protected override void ConfigureInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
            {
                base.ConfigureInputQueueEndpoint(configurator);

                _completed = Handled<JobCompleted>(configurator);

                configurator.Turnout<ProcessFile>(_busFactoryConfigurator, x =>
                {
                    x.SuperviseInterval = TimeSpan.FromSeconds(1);
                    x.SetJobFactory(async context => await Task.Delay(context.Message.Size));
                });
            }
        }

        [TestFixture]
        public class Creating_a_turnout_job_that_is_long :
            ServiceBusTestFixture
        {
            [Test]
            public async Task Should_allow_scheduling_a_job()
            {
                await Bus.Publish(new ProcessFile
                {
                    Filename = "log.txt",
                    Size = 20000
                });

                var context = await _completed;
            }

            IServiceBusBusFactoryConfigurator _busFactoryConfigurator;
            Task<ConsumeContext<JobCompleted>> _completed;

            protected override void ConfigureBus(IServiceBusBusFactoryConfigurator configurator)
            {
                base.ConfigureBus(configurator);

                _busFactoryConfigurator = configurator;
            }

            protected override void ConfigureInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
            {
                base.ConfigureInputQueueEndpoint(configurator);

                _completed = Handled<JobCompleted>(configurator);

                configurator.Turnout<ProcessFile>(_busFactoryConfigurator, x =>
                {
                    x.SuperviseInterval = TimeSpan.FromSeconds(1);
                    x.SetJobFactory(async context =>
                    {
                        await Console.Out.WriteLineAsync($"Started: {context.Message.Filename}");

                        await Task.Delay(context.Message.Size);

                        await Console.Out.WriteLineAsync($"Stopped: {context.Message.Filename}");
                    });
                });
            }
        }


        public class ProcessFile
        {
            public string Filename { get; set; }
            public int Size { get; set; }
        }
    }
}