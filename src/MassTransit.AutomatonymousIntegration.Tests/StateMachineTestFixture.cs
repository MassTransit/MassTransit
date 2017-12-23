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
namespace MassTransit.AutomatonymousIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    public class StateMachineTestFixture :
        InMemoryTestFixture
    {
        Uri _quartzQueueAddress;
        ISendEndpoint _quartzQueueSendEndpoint;

        public StateMachineTestFixture()
        {
            _quartzQueueAddress = new Uri("loopback://localhost/quartz");
        }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint QuartzQueueSendEndpoint
        {
            get { return _quartzQueueSendEndpoint; }
        }

        protected Uri QuartzQueueAddress
        {
            get { return _quartzQueueAddress; }
            set
            {
                if (Bus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _quartzQueueAddress = value;
            }
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseInMemoryScheduler();
        }

        [OneTimeSetUp]
        public async Task Setup_quartz_service()
        {
            _quartzQueueSendEndpoint = await GetSendEndpoint(_quartzQueueAddress);
        }
    }
}