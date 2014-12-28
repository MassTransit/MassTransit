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
    using TestDecorators;


    public class ConsumerTestSubjectImpl<TScenario, TSubject> :
        ConsumerTestSubject<TSubject>
        where TSubject : class, IConsumer
        where TScenario : IBusEndpointTestScenario
    {
        readonly IConsumerFactory<TSubject> _consumerFactory;
        readonly ReceivedMessageList _received;
        ConnectHandle _consumerHandler;

        public ConsumerTestSubjectImpl(TScenario scenario, IConsumerFactory<TSubject> consumerFactory)
        {
            _consumerFactory = consumerFactory;

            _received = new ReceivedMessageList(scenario.Timeout);
        }

        public IReceivedMessageList Received
        {
            get { return _received; }
        }

        public void Dispose()
        {
            if (_consumerHandler != null)
            {
                _consumerHandler.Dispose();
                _consumerHandler = null;
            }
        }

        public void Prepare(TScenario scenario)
        {
            var decoratedConsumerFactory = new TestConsumerFactoryDecorator<TSubject>(_consumerFactory, _received);

            _consumerHandler = scenario.Bus.ConnectConsumer(decoratedConsumerFactory);
        }
    }
}