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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Pipes;
    using Util;


    /// <summary>
    /// Converts a ConsumeContext to a message type and passes the context to 
    /// the output pipe. Supports interception by message type.
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class MessageConsumeFilter<TMessage> :
        IFilter<ConsumeContext>,
        IConsumePipeConnector<TMessage>,
        IRequestPipeConnector<TMessage>,
        IConsumeMessageObserverConnector
        where TMessage : class
    {
        readonly ConsumeMessageObservable<TMessage> _messageObservers;
        readonly TeeConsumeFilter<TMessage> _output;
        readonly IPipe<ConsumeContext<TMessage>> _outputPipe;

        public MessageConsumeFilter(IEnumerable<IFilter<ConsumeContext<TMessage>>> filters)
        {
            _output = new TeeConsumeFilter<TMessage>();

            _outputPipe = BuildOutputPipe(filters.Concat(Enumerable.Repeat(_output, 1)).ToArray());

            _messageObservers = new ConsumeMessageObservable<TMessage>();
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            var self = _messageObservers as ConsumeMessageObservable<T>;
            if (self == null)
                throw new InvalidOperationException("The connection type is invalid: " + TypeMetadataCache<T>.ShortName);

            return self.Connect(observer);
        }

        ConnectHandle IConsumePipeConnector<TMessage>.ConnectConsumePipe(IPipe<ConsumeContext<TMessage>> pipe)
        {
            return _output.ConnectConsumePipe(pipe);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateMessageScope(TypeMetadataCache<TMessage>.ShortName);

            _outputPipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        async Task IFilter<ConsumeContext>.Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            ConsumeContext<TMessage> consumeContext;
            if (context.TryGetMessage(out consumeContext))
            {
                if (_messageObservers.Count > 0)
                    await _messageObservers.PreConsume(consumeContext).ConfigureAwait(false);
                try
                {
                    await _outputPipe.Send(consumeContext).ConfigureAwait(false);

                    if (_messageObservers.Count > 0)
                        await _messageObservers.PostConsume(consumeContext).ConfigureAwait(false);

                    await next.Send(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // we can't await in a catch block, so we have to wait explicitly on this one
                    if (_messageObservers.Count > 0)
                    {
                        await _messageObservers.ConsumeFault(consumeContext, ex).ConfigureAwait(false);
                    }

                    throw;
                }
            }
        }

        ConnectHandle IRequestPipeConnector<TMessage>.ConnectRequestPipe(Guid requestId, IPipe<ConsumeContext<TMessage>> pipe)
        {
            return _output.ConnectRequestPipe(requestId, pipe);
        }

        static IPipe<ConsumeContext<TMessage>> BuildOutputPipe(IFilter<ConsumeContext<TMessage>>[] filters)
        {
            if (filters.Length == 0)
                throw new ArgumentException("There must be at least one filter, the output filter, for the output pipe");

            IPipe<ConsumeContext<TMessage>> current = new LastPipe<ConsumeContext<TMessage>>(filters[filters.Length - 1]);

            for (var i = filters.Length - 2; i >= 0; i--)
                current = new FilterPipe<ConsumeContext<TMessage>>(filters[i], current);

            return current;
        }
    }
}