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
namespace MassTransit.Testing.ActionConfigurators
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using TestActions;


    public class PublishTestActionConfigurator<TScenario, TMessage> :
        ITestActionConfigurator<TScenario>
        where TMessage : class
        where TScenario : ITestScenario
    {
        readonly Func<TScenario, IBus> _busAccessor;
        readonly Action<TScenario, PublishContext<TMessage>> _callback;
        readonly TMessage _message;

        public PublishTestActionConfigurator(Func<TScenario, IBus> busAccessor, TMessage message)
        {
            _busAccessor = busAccessor;
            _message = message;
        }

        public PublishTestActionConfigurator(Func<TScenario, IBus> busAccessor, TMessage message,
            Action<TScenario, PublishContext<TMessage>> callback)
        {
            _busAccessor = busAccessor;
            _message = message;
            _callback = callback;
        }

        public IEnumerable<TestConfiguratorResult> Validate()
        {
            if (_message == null)
                yield return this.Failure("Message", "The message instance to send must not be null.");
        }

        public void Configure(ITestBuilder<TScenario> builder)
        {
            var action = new PublishTestAction<TScenario, TMessage>(_busAccessor, _message, _callback);

            builder.AddTestAction(action);
        }
    }
}