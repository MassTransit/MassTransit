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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Pipes;
    using Observables;
    using Util;


    /// <summary>
    /// Maintains the Publish middleware for a message type, as well as the observers for the Publish
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class MessagePublishFilter<TMessage> :
        IFilter<PublishContext>,
        IPublishMessageObserverConnector
        where TMessage : class
    {
        readonly PublishMessageObservable<TMessage> _messageObservers;
        readonly IPipe<PublishContext<TMessage>> _outputPipe;

        public MessagePublishFilter(IEnumerable<IFilter<PublishContext<TMessage>>> filters)
        {
            _outputPipe = BuildFilterPipe(filters.ToArray());

            _messageObservers = new PublishMessageObservable<TMessage>();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateMessageScope(TypeMetadataCache<TMessage>.ShortName);

            _outputPipe.Probe(scope);
        }

        public ConnectHandle ConnectPublishMessageObserver<T>(IPublishMessageObserver<T> observer)
            where T : class
        {
            var self = _messageObservers as PublishMessageObservable<T>;
            if (self == null)
                throw new InvalidOperationException("The connection type is invalid: " + TypeMetadataCache<T>.ShortName);

            return self.Connect(observer);
        }

        [DebuggerNonUserCode]
        async Task IFilter<PublishContext>.Send(PublishContext context, IPipe<PublishContext> next)
        {
            if (context is PublishContext<TMessage> publishContext)
            {
                if (_messageObservers.Count > 0)
                    await _messageObservers.PrePublish(publishContext).ConfigureAwait(false);
                try
                {
                    await _outputPipe.Send(publishContext).ConfigureAwait(false);

                    if (_messageObservers.Count > 0)
                        await _messageObservers.PostPublish(publishContext).ConfigureAwait(false);

                    await next.Send(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (_messageObservers.Count > 0)
                    {
                        await _messageObservers.PublishFault(publishContext, ex).ConfigureAwait(false);
                    }

                    throw;
                }
            }
        }

        static IPipe<PublishContext<TMessage>> BuildFilterPipe(IFilter<PublishContext<TMessage>>[] filters)
        {
            if (filters.Length == 0)
                throw new ArgumentException("There must be at least one filter, the output filter, for the output pipe");

            IPipe<PublishContext<TMessage>> current = new LastPipe<PublishContext<TMessage>>(filters[filters.Length - 1]);

            for (int i = filters.Length - 2; i >= 0; i--)
                current = new FilterPipe<PublishContext<TMessage>>(filters[i], current);

            return current;
        }
    }
}