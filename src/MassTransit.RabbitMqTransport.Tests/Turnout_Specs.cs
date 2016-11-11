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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Turnout.Contracts;


    [TestFixture]
    public class Creating_a_turnout_job :
        RabbitMqTestFixture
    {
        [Test]
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

        protected override void ConfigureBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            configurator.UseDelayedExchangeMessageScheduler();

            base.ConfigureBusHost(configurator, host);

            configurator.TurnoutEndpoint<ProcessFile>(host, "process_queue", endpoint =>
            {
                endpoint.SuperviseInterval = TimeSpan.FromSeconds(1);
                endpoint.SetJobFactory(async context => await Task.Delay(TimeSpan.FromSeconds(context.Message.Size)).ConfigureAwait(false));

                _commandEndpointAddress = endpoint.InputAddress;
            });
        }

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _completed = Handled<JobCompleted>(configurator, context => context.Message.GetArguments<ProcessFile>().Size == 1);
            _completed2 = Handled<JobCompleted>(configurator, context => context.Message.GetArguments<ProcessFile>().Size == 2);
        }
    }
}