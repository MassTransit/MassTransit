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
namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class SendByConvention_Specs :
        InMemoryTestFixture
    {
        Task<ConsumeContext<NastyMessage>> _handled;

        [Test]
        public async Task Should_send_by_convention_to_the_input_queue()
        {
            await Bus.Send(new NastyMessage() {Value = "Hello"});

            await _handled;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<NastyMessage>(configurator);

            EndpointConvention.Map<NastyMessage>(configurator.InputAddress);
        }


        class NastyMessage
        {
            public string Value { get; set; }
        }
    }
}