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
    using Configuration;
    using NUnit.Framework;


    [TestFixture]
    public class Subscribing_two_handlers_to_the_same_topic :
        AzureServiceBusTestFixture
    {
        [Test]
        public void Should_not_create_duplicate_subscriptions()
        {
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            Handled<MessageA>(configurator);
            Handled<MessageA>(configurator);
        }


        public interface MessageA
        {
        }
    }
}