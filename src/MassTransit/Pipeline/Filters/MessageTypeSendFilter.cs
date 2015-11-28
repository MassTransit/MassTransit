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
    using System.Linq;
    using System.Threading.Tasks;
    using Util;


    public class MessageTypeSendFilter :
        IFilter<SendContext>,
        ISendObserverConnector,
        ISendMessageObserverConnector
    {
        readonly SendObservable _observers;
        readonly ConcurrentDictionary<Type, IMessagePipe> _pipes;

        public MessageTypeSendFilter()
        {
            _pipes = new ConcurrentDictionary<Type, IMessagePipe>();
            _observers = new SendObservable();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            foreach (IMessagePipe pipe in _pipes.Values)
                pipe.Filter.Probe(context);
        }

        [DebuggerNonUserCode]
        public Task Send(SendContext context, IPipe<SendContext> next)
        {
            return Task.WhenAll(_pipes.Values.Select(x => x.Filter.Send(context, next)));
        }

        public ConnectHandle ConnectSendMessageObserver<T>(ISendMessageObserver<T> observer)
            where T : class
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            ISendMessageObserverConnector messagePipe = GetPipe<T, ISendMessageObserverConnector>();

            return messagePipe.ConnectSendMessageObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        TResult GetPipe<T, TResult>()
            where T : class
            where TResult : class
        {
            return GetPipe<T>().As<TResult>();
        }

        IMessagePipe GetPipe<T>()
            where T : class
        {
            return _pipes.GetOrAdd(typeof(T), x => new MessagePipe<T>(_observers));
        }

        public void AddFilter<T>(IFilter<SendContext<T>> filter)
            where T : class
        {
            GetPipe<T>().AddFilter(filter);
        }


        interface IMessagePipe
        {
            IFilter<SendContext> Filter { get; }

            TResult As<TResult>()
                where TResult : class;

            void AddFilter<T>(IFilter<SendContext<T>> filter)
                where T : class;
        }


        class MessagePipe<TMessage> :
            IMessagePipe,
            ISendMessageObserver<TMessage>
            where TMessage : class
        {
            readonly Lazy<IFilter<SendContext>> _filter;
            readonly SendObservable _observer;
            readonly IList<IFilter<SendContext<TMessage>>> _pipeFilters;

            public MessagePipe(SendObservable observer)
            {
                _filter = new Lazy<IFilter<SendContext>>(CreateFilter);
                _observer = observer;

                _pipeFilters = new List<IFilter<SendContext<TMessage>>>();
            }

            public IFilter<SendContext> Filter => _filter.Value;

            TResult IMessagePipe.As<TResult>()
            {
                return _filter.Value as TResult;
            }

            void IMessagePipe.AddFilter<T>(IFilter<SendContext<T>> filter)
            {
                if (_filter.IsValueCreated)
                    throw new ConfigurationException("The filter has already been created, no additional filters can be added");

                var self = this as MessagePipe<T>;
                if (self == null)
                    throw new ArgumentException("The message type is invalid: " + TypeMetadataCache<T>.ShortName);

                self._pipeFilters.Add(filter);
            }

            public Task PreSend(SendContext<TMessage> context)
            {
                return _observer.PreSend(context);
            }

            public Task PostSend(SendContext<TMessage> context)
            {
                return _observer.PostSend(context);
            }

            public Task SendFault(SendContext<TMessage> context, Exception exception)
            {
                return _observer.SendFault(context, exception);
            }

            IFilter<SendContext> CreateFilter()
            {
                var messageSendFilter = new MessageSendFilter<TMessage>(_pipeFilters);

                messageSendFilter.ConnectSendMessageObserver(this);

                return messageSendFilter;
            }
        }
    }
}