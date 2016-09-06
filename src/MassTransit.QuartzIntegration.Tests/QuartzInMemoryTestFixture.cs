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


    public class QuartzInMemoryTestFixture :
        InMemoryTestFixture
    {
        ISendEndpoint _quartzEndpoint;

        public QuartzInMemoryTestFixture()
        {
            QuartzAddress = new Uri("loopback://localhost/quartz");
        }

        protected Uri QuartzAddress { get; }

        protected ISendEndpoint QuartzEndpoint => _quartzEndpoint;

        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureBus(configurator);

            configurator.UseInMemoryScheduler();
        }

        [OneTimeSetUp]
        public async Task Setup_quartz_service()
        {
            _quartzEndpoint = await GetSendEndpoint(QuartzAddress);
        }
    }
}