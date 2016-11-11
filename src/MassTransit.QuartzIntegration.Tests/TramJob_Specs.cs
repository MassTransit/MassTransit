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
    using TestFramework;
    using Turnout.Contracts;


    [TestFixture]
    public class TramJob_Specs :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_allow_scheduling_a_job()
        {
            var completed = SubscribeHandler<JobCompleted>();

            await Bus.Publish(new ProcessFile
            {
                Filename = "log.txt",
                Size = 10
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
//                x.SetJobFactory(async context => await Task.Delay(context.Message.Size));
//            });
        }


        public class ProcessFile
        {
            public string Filename { get; set; }
            public int Size { get; set; }
        }
    }


    [TestFixture]
    public class When_a_job_faults :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_job_faulted_event()
        {
            var faulted = SubscribeHandler<JobFaulted>();

            await Bus.Publish(new ProcessFile
            {
                Filename = "log.txt",
                Size = 10
            });

            var context = await faulted;
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
//                    await Task.Delay(context.Message.Size);
//
//                    throw new IntentionalTestException();
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