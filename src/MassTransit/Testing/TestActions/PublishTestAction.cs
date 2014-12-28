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
namespace MassTransit.Testing.TestActions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Scenarios;


    public class PublishTestAction<TScenario, TMessage> :
        ITestAction<TScenario>
        where TMessage : class
        where TScenario : ITestScenario
    {
        readonly Func<TScenario, IPublishEndpoint> _busAccessor;
        readonly Action<TScenario, PublishContext<TMessage>> _callback;
        readonly TMessage _message;

        public PublishTestAction(Func<TScenario, IPublishEndpoint> busAccessor, TMessage message,
            Action<TScenario, PublishContext<TMessage>> callback)
        {
            _message = message;
            _busAccessor = busAccessor;
            _callback = callback ?? DefaultCallback;
        }

        public Task Act(TScenario scenario, CancellationToken cancellationToken)
        {
            IPublishEndpoint publishEndpoint = _busAccessor(scenario);

            return publishEndpoint.Publish(_message,
                Pipe.New<PublishContext<TMessage>>(x => x.Execute(context => _callback(scenario, context))), cancellationToken);
        }

        static void DefaultCallback(TScenario scenario, PublishContext<TMessage> context)
        {
        }
    }
}