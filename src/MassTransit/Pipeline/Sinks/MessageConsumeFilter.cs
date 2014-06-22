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
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// Converts a ConsumeContext to a message type and passes the context to 
    /// the output pipe. Supports interception by message type.
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class MessageConsumeFilter<TMessage> :
        IConsumeFilter,
        IConsumeFilterConnector,
        IConsumeObserverConnector
        where TMessage : class
    {
        readonly ObserverConnectable<TMessage> _connections;
        readonly TeeConsumeFilter<TMessage> _output;

        public MessageConsumeFilter()
        {
            _output = new TeeConsumeFilter<TMessage>();
            _connections = new ObserverConnectable<TMessage>();
        }

        async Task IFilter<ConsumeContext>.Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            ConsumeContext<TMessage> consumeContext;
            if (context.TryGetMessage(out consumeContext))
            {
                if (_connections.Count > 0)
                    await _connections.PreDispatch(consumeContext);

                try
                {
                    await _output.Send(consumeContext, next);

                    if (_connections.Count > 0)
                        await _connections.PostDispatch(consumeContext);
                }
                catch (Exception ex)
                {
                    // we can't await in a catch block, so we have to wait explicitly on this one
                    if (_connections.Count > 0)
                        _connections.DispatchFaulted(consumeContext, ex).Wait(context.CancellationToken);

                    throw;
                }
            }
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, (x, _) => _output.Inspect(x));
        }

        public ConnectHandle Connect<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            var output = _output as IConsumeFilterConnector<T>;
            if (output == null)
                throw new ArgumentException("Invalid pipe type specified: " + TypeMetadataCache<T>.ShortName);

            return output.Connect(pipe);
        }

        ConnectHandle IConsumeObserverConnector.Connect<T>(IConsumeObserver<T> observer)
        {
            var self = _connections as ObserverConnectable<T>;
            if (self == null)
                throw new InvalidOperationException("The connection type is invalid: " + TypeMetadataCache<T>.ShortName);

            return self.Connect(observer);
        }
    }
}