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
namespace MassTransit.Testing.ActionConfigurators
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using Scenarios;
    using TestActions;


    public class SendTestActionConfigurator<TScenario, TMessage> :
        ITestActionConfigurator<TScenario>
        where TMessage : class
        where TScenario : ITestScenario
    {
        readonly Action<TScenario, SendContext<TMessage>> _callback;
        readonly Func<TScenario, ISendEndpoint> _endpointAccessor;
        readonly TMessage _message;

        public SendTestActionConfigurator(Func<TScenario, ISendEndpoint> endpointAccessor, TMessage message)
        {
            _message = message;
            _endpointAccessor = endpointAccessor;
        }

        public SendTestActionConfigurator(Func<TScenario, ISendEndpoint> endpointAccessor, TMessage message,
            Action<TScenario, SendContext<TMessage>> callback)
        {
            _message = message;
            _callback = callback;
            _endpointAccessor = endpointAccessor;
        }

        public IEnumerable<TestConfiguratorResult> Validate()
        {
            if (_message == null)
                yield return this.Failure("Message", "The message instance to send must not be null.");
        }

        public void Configure(TestInstanceBuilder<TScenario> builder)
        {
            var action = new SendTestAction<TScenario, TMessage>(_endpointAccessor, _message, _callback);

            builder.AddTestAction(action);
        }
    }
}