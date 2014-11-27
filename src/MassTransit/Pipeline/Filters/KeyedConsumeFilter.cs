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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;


    public class KeyedConsumeFilter<T, TKey> :
        IFilter<ConsumeContext<T>>,
        IConnectPipeById<ConsumeContext<T>, TKey>
        where T : class
    {
        readonly KeyAccessor<ConsumeContext<T>, TKey> _keyAccessor;
        readonly ConcurrentDictionary<TKey, TeeConsumeFilter<T>> _pipes;

        public KeyedConsumeFilter(KeyAccessor<ConsumeContext<T>, TKey> keyAccessor)
        {
            _keyAccessor = keyAccessor;
            _pipes = new ConcurrentDictionary<TKey, TeeConsumeFilter<T>>();
        }

        public ConnectHandle Connect(TKey key, IPipe<ConsumeContext<T>> pipe)
        {
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            TeeConsumeFilter<T> added = _pipes.GetOrAdd(key, x => new TeeConsumeFilter<T>());

            ConnectHandle handle = added.Connect(pipe);

            return new Handle(key, handle, RemovePipe);
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            TKey key = _keyAccessor(context);

            TeeConsumeFilter<T> filter;
            if (_pipes.TryGetValue(key, out filter))
                await filter.Send(context, next);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, x => _pipes.Values.Cast<IFilter<ConsumeContext<T>>>().All(pipe => pipe.Inspect(x)));
        }

        void RemovePipe(TKey key, ConnectHandle connectHandle)
        {
            connectHandle.Disconnect();

            TeeConsumeFilter<T> filter;
            if (_pipes.TryGetValue(key, out filter) && filter.Count == 0)
            {
                TeeConsumeFilter<T> removedFilter;
                if (_pipes.TryRemove(key, out removedFilter))
                {
                    if (removedFilter.Count > 0)
                        throw new InvalidOperationException("Keys must not be reused");
                }
            }
        }


        class Handle :
            ConnectHandle
        {
            readonly ConnectHandle _handle;
            readonly TKey _key;
            readonly Action<TKey, ConnectHandle> _removeKey;

            public Handle(TKey key, ConnectHandle handle, Action<TKey, ConnectHandle> removeKey)
            {
                _key = key;
                _handle = handle;
                _removeKey = removeKey;
            }

            public void Disconnect()
            {
                _removeKey(_key, _handle);
            }

            public void Dispose()
            {
                Disconnect();
            }
        }
    }
}