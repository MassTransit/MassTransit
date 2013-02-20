// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Configuration
{
    using System;
    using Magnum.Extensions;
    using Magnum.TestFramework;

    [Scenario]
    public class When_using_default_settings
    {
        IServiceBus _bus;

        [When]
         public void When_creating_a_bus_with_defaults()
        {
            _bus = ServiceBusFactory.New(x => x.ReceiveFrom("loopback://localhost/mt_test"));
        }

        [Then]
        public void Should_have_shutdown_timeout_of_60_seconds()
        {
            _bus.ShutdownTimeout.ShouldEqual(60.Seconds());
        }

        [Finally]
        public void Finally()
        {
            _bus.Dispose();
        }
    }

    [Scenario]
    public class When_using_custom_timeout
    {
        IServiceBus _bus;
        TimeSpan _timeout;

        [When]
        public void When_creating_a_bus_with_and_configuring_a_custom_shutdown_timeout()
        {
            _timeout = 90.Seconds();
            _bus = ServiceBusFactory.New(x =>
            {
                x.ReceiveFrom("loopback://localhost/mt_test");
                x.SetShutdownTimeout(_timeout);
            });
        }

        [Then]
        public void Should_have_the_correct_shutdown_timeout()
        {
            _bus.ShutdownTimeout.ShouldEqual(_timeout);
        }

        [Finally]
        public void Finally()
        {
            _bus.Dispose();
        }
    }
}