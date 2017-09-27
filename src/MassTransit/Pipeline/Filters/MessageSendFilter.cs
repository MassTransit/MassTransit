// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Observables;
    using Util;


    /// <summary>
    /// Maintains the send middleware for a message type, as well as the observers for the send
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class MessageSendFilter<TMessage> :
        IFilter<SendContext>,
        ISendMessageObserverConnector
        where TMessage : class
    {
        readonly SendMessageObservable<TMessage> _messageObservers;
        readonly IPipe<SendContext<TMessage>> _outputPipe;

        public MessageSendFilter(IEnumerable<IFilter<SendContext<TMessage>>> filters)
        {
            _outputPipe = BuildFilterPipe(filters.ToArray());

            _messageObservers = new SendMessageObservable<TMessage>();
        }

        [DebuggerNonUserCode]
        async Task IFilter<SendContext>.Send(SendContext context, IPipe<SendContext> next)
        {
            if (context is SendContext<TMessage> sendContext)
            {
                if (_messageObservers.Count > 0)
                    await _messageObservers.PreSend(sendContext).ConfigureAwait(false);
                try
                {
                    await _outputPipe.Send(sendContext).ConfigureAwait(false);

                    if (_messageObservers.Count > 0)
                        await _messageObservers.PostSend(sendContext).ConfigureAwait(false);

                    await next.Send(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (_messageObservers.Count > 0)
                    {
                        await _messageObservers.SendFault(sendContext, ex).ConfigureAwait(false);
                    }

                    throw;
                }
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateMessageScope(TypeMetadataCache<TMessage>.ShortName);

            _outputPipe.Probe(scope);
        }

        public ConnectHandle ConnectSendMessageObserver<T>(ISendMessageObserver<T> observer)
            where T : class
        {
            var self = _messageObservers as SendMessageObservable<T>;
            if (self == null)
                throw new InvalidOperationException("The connection type is invalid: " + TypeMetadataCache<T>.ShortName);

            return self.Connect(observer);
        }

        static IPipe<SendContext<TMessage>> BuildFilterPipe(IFilter<SendContext<TMessage>>[] filters)
        {
            if (filters.Length == 0)
                throw new ArgumentException("There must be at least one filter, the output filter, for the output pipe");

            IPipe<SendContext<TMessage>> current = new LastPipe<SendContext<TMessage>>(filters[filters.Length - 1]);

            for (var i = filters.Length - 2; i >= 0; i--)
                current = new FilterPipe<SendContext<TMessage>>(filters[i], current);

            return current;
        }
    }
}