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

    public class CorrelatedResponseAction<TMessage, TKey> :
        IResponseAction,
        Consumes<TMessage>.For<TKey>
        where TMessage : class
    {
        readonly TKey _correlationId;
        readonly Action<TMessage> _responseAction;
        readonly RequestResponseScope _scope;

        public CorrelatedResponseAction(RequestResponseScope scope, TKey correlationId, Action<TMessage> responseAction)
        {
            _scope = scope;
            _correlationId = correlationId;
            _responseAction = responseAction;
        }

        public void Consume(TMessage message)
        {
            _responseAction(message);

            _scope.SetResponseReceived(message);
        }

        public TKey CorrelationId
        {
            get { return _correlationId; }
        }

        public UnsubscribeAction SubscribeTo(IServiceBus bus)
        {
            return bus.SubscribeInstance(this);
        }
    }
}