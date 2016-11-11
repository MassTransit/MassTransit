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
    using Turnout.Contracts;


    [TestFixture]
    public class A_turnout_check_consumer :
        QuartzInMemoryTestFixture
    {
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

        IInMemoryBusFactoryConfigurator _busFactoryConfigurator;

        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureBus(configurator);

            _busFactoryConfigurator = configurator;
        }

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInputQueueEndpoint(configurator);

//            configurator.Turnout<ProcessFile>(_busFactoryConfigurator, x =>
//            {
//                x.SuperviseInterval = TimeSpan.FromSeconds(1);
//                x.SetJobFactory(async context =>
//                {
//                    await Console.Out.WriteLineAsync($"Started: {context.Message.Filename}");
//
//                    await Task.Delay(context.Message.Size);
//
//                    await Console.Out.WriteLineAsync($"Stopped: {context.Message.Filename}");
//                });
//            });
        }


        public class ProcessFile
        {
            public string Filename { get; set; }
            public int Size { get; set; }
        }
    }
}