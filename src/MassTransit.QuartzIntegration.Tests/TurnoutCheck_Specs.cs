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
    using Turnout;
    using Turnout.Contracts;


    [TestFixture]
    public class A_turnout_check_consumer :
        QuartzInMemoryTestFixture
    {
        public A_turnout_check_consumer()
        {
            _checkInterval = TimeSpan.FromSeconds(1);

            _roster = new JobRoster();
        }

        ConsumerTurnout _consumerTurnout;
        readonly TimeSpan _checkInterval;
        Uri _controlAddress;
        readonly JobRoster _roster;

        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureBus(configurator);

            configurator.ReceiveEndpoint("turnout_control", e =>
            {
                _controlAddress = e.InputAddress;

                e.Consumer(() => new TurnoutJobConsumer(_roster, _checkInterval));
            });
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInputQueueEndpoint(configurator);

            IJobFactory<ProcessFile> jobFactory = new DelegateJobFactory<ProcessFile>(async context =>
            {
                await Console.Out.WriteLineAsync("Started");

                await Task.Delay(context.Message.Size);

                await Console.Out.WriteLineAsync("Stopped");
            });

            _consumerTurnout = new ConsumerTurnout(_roster, _controlAddress, _checkInterval);

            configurator.Consumer(() => new CreateJobConsumer<ProcessFile>(_consumerTurnout, jobFactory));
        }


        public class ProcessFile
        {
            public string Filename { get; set; }
            public int Size { get; set; }
        }


        [Test]
        public async Task Should_find_existing_jobs_and_continue_to_check_them()
        {
            var completed = SubscribeHandler<JobCompleted>();

            await Bus.Publish(new ProcessFile
            {
                Filename = "log.txt",
                Size = 10000
            });

            var context = await completed;
        }
    }
}