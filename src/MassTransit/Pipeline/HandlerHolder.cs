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
namespace MassTransit.Pipeline
{
    using System;
    using System.Collections.Concurrent;


    public interface Rail<T>
    {
    }


    public class RequestSubscriptionRouter<T>
        where T : class
    {
        readonly ConcurrentDictionary<Guid, Rail<T>> _observers;

        public RequestSubscriptionRouter()
        {
            _observers = new ConcurrentDictionary<Guid, Rail<T>>();
        }

        public IDisposable Subscribe(Guid requestId, Rail<T> rail)
        {
            if (requestId == Guid.Empty)
                throw new ArgumentException("The RequestId must be initialized", "requestId");
            if (rail == null)
                throw new ArgumentNullException("rail");

            var added = _observers.TryAdd(requestId, rail);
            if (!added)
                throw new InvalidOperationException("The RequestId is already registered: " + requestId);

            return new SubscriberReference(requestId, id =>
                {
                    Rail<T> removedRail;
                    if (!_observers.TryRemove(id, out removedRail))
                        throw new InvalidOperationException("The RequestId was not registered: " + id);
                });
        }


        class SubscriberReference :
            IDisposable
        {
            readonly Action<Guid> _removeRail;
            readonly Guid _requestId;

            public SubscriberReference(Guid requestId, Action<Guid> removeRail)
            {
                _requestId = requestId;
                _removeRail = removeRail;
            }

            public void Dispose()
            {
                _removeRail(_requestId);
            }
        }
    }
}