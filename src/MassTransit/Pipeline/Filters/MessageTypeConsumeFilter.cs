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
    using Monitoring.Introspection;
    using Util;


    public class MessageTypeConsumeFilter :
        IFilter<ConsumeContext>,
        IConsumePipeConnector,
        IRequestPipeConnector,
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector
    {
        readonly ConsumeObserverConnectable _observers;
        readonly ConcurrentDictionary<Type, IMessagePipe> _pipes;

        public MessageTypeConsumeFilter()
        {
            _pipes = new ConcurrentDictionary<Type, IMessagePipe>();
            _observers = new ConsumeObserverConnectable();
        }

        async Task IProbeSite.Probe(ProbeContext context)
        {
            foreach (var pipe in _pipes.Values)
            {
                await pipe.Filter.Probe(context);
            }
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            IConsumeMessageObserverConnector messagePipe = GetPipe<T, IConsumeMessageObserverConnector>();

            return messagePipe.ConnectConsumeMessageObserver(observer);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _observers.Connect(observer);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            IConsumePipeConnector<T> messagePipe = GetPipe<T, IConsumePipeConnector<T>>();

            return messagePipe.ConnectConsumePipe(pipe);
        }

        [DebuggerNonUserCode]
        public Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            return Task.WhenAll(_pipes.Values.Select(x => x.Filter.Send(context, next)));
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            IRequestPipeConnector<T> messagePipe = GetPipe<T, IRequestPipeConnector<T>>();

            return messagePipe.ConnectRequestPipe(requestId, pipe);
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

        public void AddFilter<T>(IFilter<ConsumeContext<T>> filter)
            where T : class
        {
            GetPipe<T>().AddFilter(filter);
        }


        interface IMessagePipe 
        {
            IFilter<ConsumeContext> Filter { get; }

            TResult As<TResult>()
                where TResult : class;

            void AddFilter<T>(IFilter<ConsumeContext<T>> filter)
                where T : class;
        }


        class MessagePipe<TMessage> :
            IMessagePipe,
            IConsumeMessageObserver<TMessage>
            where TMessage : class
        {
            readonly Lazy<IFilter<ConsumeContext>> _filter;
            readonly IConsumeObserver _observer;
            readonly IList<IFilter<ConsumeContext<TMessage>>> _pipeFilters;

            public MessagePipe(IConsumeObserver observer)
            {
                _filter = new Lazy<IFilter<ConsumeContext>>(CreateFilter);
                _observer = observer;

                _pipeFilters = new List<IFilter<ConsumeContext<TMessage>>>();
            }

            public Task PreConsume(ConsumeContext<TMessage> context)
            {
                return _observer.PreConsume(context);
            }

            public Task PostConsume(ConsumeContext<TMessage> context)
            {
                return _observer.PostConsume(context);
            }

            public Task ConsumeFault(ConsumeContext<TMessage> context, Exception exception)
            {
                return _observer.ConsumeFault(context, exception);
            }

            public IFilter<ConsumeContext> Filter
            {
                get { return _filter.Value; }
            }

            TResult IMessagePipe.As<TResult>()
            {
                return _filter.Value as TResult;
            }

            void IMessagePipe.AddFilter<T>(IFilter<ConsumeContext<T>> filter)
            {
                if (_filter.IsValueCreated)
                    throw new ConfigurationException("The filter has already been created, no additional filters can be added");

                var self = this as MessagePipe<T>;
                if (self == null)
                    throw new ArgumentException("The message type is invalid: " + TypeMetadataCache<T>.ShortName);

                self._pipeFilters.Add(filter);
            }

            IFilter<ConsumeContext> CreateFilter()
            {
                var messageConsumeFilter = new MessageConsumeFilter<TMessage>(_pipeFilters);

                // we subscribe to any events so that they are pushed up the stack
                ((IConsumeMessageObserverConnector)messageConsumeFilter).ConnectConsumeMessageObserver(this);

                return messageConsumeFilter;
            }
        }
    }
}