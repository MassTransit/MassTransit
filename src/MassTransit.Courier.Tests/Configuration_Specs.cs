// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Courier.Tests
{
    using System;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    [TestFixture]
    public class When_configuring_activity_hosts_with_masstransit :
        InMemoryTestFixture
    {
        [Test]
        public void Should_have_a_clean_interface()
        {
        }

        public When_configuring_activity_hosts_with_masstransit()
        {
            _executeUri = new Uri("loopback://localhost/execute_testactivity");
            _compensateUri = new Uri("loopback://localhost/compensate_testactivity");
        }

        readonly Uri _executeUri;
        readonly Uri _compensateUri;

        protected override void ConfigureBus(IInMemoryServiceBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("execute_testactivity", x =>
            {
                x.ExecuteActivityHost<TestActivity, TestArguments>(_compensateUri);
            });

            configurator.ReceiveEndpoint("compensate_testactivity", x =>
            {
                x.CompensateActivityHost<TestActivity, TestLog>();
            });
        }
    }
}