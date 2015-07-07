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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles the registration of requests and connecting them to the consume pipe
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class RequestConsumeFilter<T, TKey> :
        IFilter<ConsumeContext<T>>,
        IConnectPipeById<ConsumeContext<T>, TKey>
        where T : class
    {
        readonly KeyAccessor<ConsumeContext<T>, TKey> _keyAccessor;
        readonly ConcurrentDictionary<TKey, RequestPipeFilter<T, TKey>> _pipes;

        public RequestConsumeFilter(KeyAccessor<ConsumeContext<T>, TKey> keyAccessor)
        {
            _keyAccessor = keyAccessor;
            _pipes = new ConcurrentDictionary<TKey, RequestPipeFilter<T, TKey>>();
        }

        public ConnectHandle Connect(TKey key, IPipe<ConsumeContext<T>> pipe)
        {
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            bool added = _pipes.TryAdd(key, new RequestPipeFilter<T, TKey>(key, pipe));
            if (!added)
                throw new RequestException("A consumer for the requestId is already connected");

            return new Handle(key, RemovePipe);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("request");

            ICollection<RequestPipeFilter<T, TKey>> filters = _pipes.Values;
            scope.Add("count", filters.Count);

            foreach (IProbeSite filter in filters)
                filter.Probe(scope);
        }

        [DebuggerNonUserCode]
        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            TKey key = _keyAccessor(context);

            RequestPipeFilter<T, TKey> filter;
            if (_pipes.TryGetValue(key, out filter))
                await filter.Send(context, next);
        }

        void RemovePipe(TKey key)
        {
            RequestPipeFilter<T, TKey> filter;
            _pipes.TryRemove(key, out filter);
        }


        class Handle :
            ConnectHandle
        {
            readonly TKey _key;
            readonly Action<TKey> _removeKey;

            public Handle(TKey key, Action<TKey> removeKey)
            {
                _key = key;
                _removeKey = removeKey;
            }

            public void Disconnect()
            {
                _removeKey(_key);
            }

            public void Dispose()
            {
                Disconnect();
            }
        }
    }
}