// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RequestResponse
{
    using System;

    public class ResponseActionBuilder<TMessage>
        where TMessage : class
    {
        readonly RequestResponseScope _scope;

        public ResponseActionBuilder(RequestResponseScope scope)
        {
            _scope = scope;
        }

        public CorrelatedResponseActionBuilder<TMessage, TKey> RelatedTo<TKey>(TKey correlationId)
        {
            return new CorrelatedResponseActionBuilder<TMessage, TKey>(_scope, correlationId);
        }

        public ConditionalResponseActionBuilder<TMessage> And(Func<TMessage, bool> accept)
        {
            return new ConditionalResponseActionBuilder<TMessage>(_scope, accept);
        }

        public RequestResponseScope IsReceived(Action<TMessage> action)
        {
            _scope.AddResponseAction(new ResponseAction<TMessage>(_scope, action));

            return _scope;
        }
    }
}