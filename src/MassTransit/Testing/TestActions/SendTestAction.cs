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
namespace MassTransit.Testing.TestActions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class SendTestAction<TScenario, TMessage> :
        ITestAction<TScenario>
        where TMessage : class
        where TScenario : ITestScenario
    {
        readonly Action<TScenario, SendContext<TMessage>> _callback;
        readonly Func<TScenario, Task<ISendEndpoint>> _endpointAccessor;
        readonly TMessage _message;

        public SendTestAction(Func<TScenario, Task<ISendEndpoint>> endpointAccessor, TMessage message,
            Action<TScenario, SendContext<TMessage>> callback)
        {
            _message = message;
            _endpointAccessor = endpointAccessor;
            _callback = callback ?? DefaultCallback;
        }

        public async Task Act(TScenario scenario, CancellationToken cancellationToken)
        {
            var endpoint = await _endpointAccessor(scenario).ConfigureAwait(false);

            await endpoint.Send(_message, context => _callback(scenario, context), cancellationToken).ConfigureAwait(false);
        }

        static void DefaultCallback(TScenario scenario, SendContext<TMessage> context)
        {
        }
    }
}