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
namespace MassTransit.Testing.Subjects
{
    using System.Collections.Generic;
    using Configurators;
    using ScenarioBuilders;
    using ScenarioConfigurators;
    using TestDecorators;


    public class ConsumerTestSubject<TScenario, TSubject> :
        IConsumerTestSubject<TSubject>,
        IScenarioBuilderConfigurator<TScenario>
        where TSubject : class, IConsumer
        where TScenario : IBusTestScenario
    {
        readonly IConsumerFactory<TSubject> _consumerFactory;
        ReceivedMessageList _received;

        public ConsumerTestSubject(IConsumerFactory<TSubject> consumerFactory)
        {
            _consumerFactory = consumerFactory;
        }

        public IReceivedMessageList Received
        {
            get { return _received; }
        }

        public void Dispose()
        {
        }

        public ITestScenarioBuilder<TScenario> Configure(ITestScenarioBuilder<TScenario> builder)
        {
            _received = new ReceivedMessageList(builder.Timeout);
            var decoratedConsumerFactory = new TestConsumerFactoryDecorator<TSubject>(_consumerFactory, _received);

            var scenarioBuilder = builder as IBusTestScenarioBuilder;
            if (scenarioBuilder != null)
                scenarioBuilder.ConfigureReceiveEndpoint(x => x.Consumer(decoratedConsumerFactory));

            return builder;
        }

        public IEnumerable<TestConfiguratorResult> Validate()
        {
            if (_consumerFactory == null)
                yield return this.Failure("ConsumerFactory", "must not be null");
        }
    }
}