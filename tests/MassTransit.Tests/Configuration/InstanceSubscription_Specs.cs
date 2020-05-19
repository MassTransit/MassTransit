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
namespace MassTransit.Tests.Configuration
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Pipeline;
    using TestFramework;
    using TestFramework.Messages;


    public class When_subscribing_an_object_instance_to_the_bus :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_received_the_message()
        {
            _message = new MessageA();

            await Bus.Publish(_message);

            await _consumer.Task;
        }

        OneMessageConsumer _consumer;
        MessageA _message;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new OneMessageConsumer(GetTask<MessageA>());

            object instance = _consumer;

            configurator.Instance(instance);
        }
    }


    public class When_subscribing_a_consumer_to_the_bus_by_factory_method :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_received_the_message()
        {
            _message = new MessageA();

            await Bus.Publish(_message);

            await _consumer.Task;
        }

        OneMessageConsumer _consumer;
        MessageA _message;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new OneMessageConsumer(GetTask<MessageA>());


            configurator.Consumer(() => _consumer);
        }
    }

    public class When_subscribing_a_consumer_to_the_bus_by_object_factory_method :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_received_the_message()
        {
            _message = new MessageA();

            await Bus.Publish(_message);

            await _consumer.Task;
        }

        OneMessageConsumer _consumer;
        MessageA _message;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new OneMessageConsumer(GetTask<MessageA>());

            configurator.Consumer(typeof(OneMessageConsumer), type => _consumer);
        }
    }
}