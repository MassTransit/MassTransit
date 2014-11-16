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
namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using EndpointConfigurators;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Using_the_in_memory_transports :
        InMemoryTestFixture
    {
        [Test]
        public void Should_properly_startup_and_shutdown()
        {
        }

        [Test]
        public async void Should_send_a_message_properly()
        {
            await LocalSendEndpoint.Send(new A());

            await _receivedA;

            Assert.IsTrue(Sent.Any<A>());
        }

        Task<A> _receivedA;


        class A
        {
        }


        protected override void ConfigureLocalReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _receivedA = Handler<A>(configurator);
        }
    }
}