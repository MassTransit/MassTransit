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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Exceptions;


    /// <summary>
    /// Connects multiple output pipes to a single input pipe
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TeeConsumeFilter<T> :
        IConsumeFilter<T>,
        IConsumeFilterConnector<T>
        where T : class
    {
        readonly ConcurrentDictionary<long, IPipe<ConsumeContext<T>>> _pipes;
        long _nextPipeId;

        public TeeConsumeFilter()
        {
            _pipes = new ConcurrentDictionary<long, IPipe<ConsumeContext<T>>>();
        }

        public int Count
        {
            get { return _pipes.Count; }
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var exceptions = new List<Exception>();

            foreach (var pipe in _pipes.Values)
            {
                try
                {
                    await pipe.Send(context);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);

            await next.Send(context);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, (x, _) => _pipes.Values.All(pipe => pipe.Inspect(x)));
        }

        public ConnectHandle Connect(params IFilter<ConsumeContext<T>>[] filters)
        {
            long pipeId = Interlocked.Increment(ref _nextPipeId);

            var pipe = filters.Combine();

            bool added = _pipes.TryAdd(pipeId, pipe);
            if (!added)
                throw new PipelineException("Unable to add pipe");

            return new TeeConnectHandle(pipeId, RemovePipe);
        }

        void RemovePipe(long id)
        {
            IPipe<ConsumeContext<T>> ignored;
            _pipes.TryRemove(id, out ignored);
        }


        class TeeConnectHandle :
            ConnectHandle
        {
            readonly long _pipeId;
            readonly Action<long> _removePipe;

            public TeeConnectHandle(long pipeId, Action<long> removePipe)
            {
                _pipeId = pipeId;
                _removePipe = removePipe;
            }

            public void Disconnect()
            {
                _removePipe(_pipeId);
            }

            public void Dispose()
            {
                Disconnect();
            }
        }
    }
}