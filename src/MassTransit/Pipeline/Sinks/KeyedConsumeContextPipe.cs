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
namespace MassTransit.Pipeline.Sinks
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;


    public class KeyedConsumeContextPipe<T, TKey> :
        IConsumeContextPipe<T>,
        IConnectPipeById<T, TKey>
        where T : class
    {
        readonly KeyAccessor<T, TKey> _keyAccessor;
        readonly ConcurrentDictionary<TKey, TeeConsumeContextPipe<T>> _pipes;

        public KeyedConsumeContextPipe(KeyAccessor<T, TKey> keyAccessor)
        {
            _keyAccessor = keyAccessor;
            _pipes = new ConcurrentDictionary<TKey, TeeConsumeContextPipe<T>>();
        }

        public ConnectHandle Connect(TKey key, IConsumeContextPipe<T> pipe)
        {
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            TeeConsumeContextPipe<T> added = _pipes.GetOrAdd(key, x => new TeeConsumeContextPipe<T>());

            ConnectHandle handle = added.Connect(pipe);

            return new Handle(key, handle, RemovePipe);
        }

        public async Task Send(ConsumeContext<T> context)
        {
            TKey key = _keyAccessor(context.Message);

            TeeConsumeContextPipe<T> pipe;
            if (_pipes.TryGetValue(key, out pipe))
                await pipe.Send(context);
        }

        public bool Inspect(IConsumeContextPipeInspector inspector)
        {
            return inspector.Inspect(this,
                (x, _) => _pipes.Values.Cast<IConsumeContextPipe<T>>().All(pipe => pipe.Inspect(x)));
        }

        void RemovePipe(TKey key, ConnectHandle connectHandle)
        {
            connectHandle.Disconnect();

            TeeConsumeContextPipe<T> pipe;
            if (_pipes.TryGetValue(key, out pipe) && pipe.Count == 0)
            {
                TeeConsumeContextPipe<T> removedPipe;
                if (_pipes.TryRemove(key, out removedPipe))
                {
                    if (removedPipe.Count > 0)
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
        }
    }
}