// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Testing
{
    using Decorators;
    using MessageObservers;


    public class ConsumerTestHarness<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly ReceivedMessageList _consumed;
        readonly IConsumerFactory<TConsumer> _consumerFactory;

        public ConsumerTestHarness(BusTestHarness testHarness, IConsumerFactory<TConsumer> consumerFactory, string queueName)
        {
            _consumerFactory = consumerFactory;

            _consumed = new ReceivedMessageList(testHarness.TestTimeout);

            if (string.IsNullOrWhiteSpace(queueName))
                testHarness.OnConfigureReceiveEndpoint += ConfigureReceiveEndpoint;
            else
                testHarness.OnConfigureBus += configurator => ConfigureNamedReceiveEndpoint(configurator, queueName);
        }

        public IReceivedMessageList Consumed => _consumed;

        protected virtual void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            var decorator = new TestConsumerFactoryDecorator<TConsumer>(_consumerFactory, _consumed);

            configurator.Consumer(decorator);
        }

        protected virtual void ConfigureNamedReceiveEndpoint(IBusFactoryConfigurator configurator, string queueName)
        {
            configurator.ReceiveEndpoint(queueName, x =>
            {
                var decorator = new TestConsumerFactoryDecorator<TConsumer>(_consumerFactory, _consumed);

                x.Consumer(decorator);
            });
        }
    }
}