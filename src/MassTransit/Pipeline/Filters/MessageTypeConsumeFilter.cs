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


    public class MessageTypeConsumeFilter :
        IFilter<ConsumeContext>,
        IConsumeFilterConnector,
        IRequestFilterConnector,
        IMessageObserverConnector,
        IConsumeObserverConnector
    {
        readonly ConsumeObserverConnectable _observers;
        readonly ConcurrentDictionary<Type, IMessageFilter> _pipes;

        public MessageTypeConsumeFilter()
        {
            _pipes = new ConcurrentDictionary<Type, IMessageFilter>();
            _observers = new ConsumeObserverConnectable();
        }

        public ConnectHandle Connect<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            IConsumeFilterConnector<T> messagePipe = GetPipe<T, IConsumeFilterConnector<T>>();

            return messagePipe.Connect(pipe);
        }

        public ConnectHandle Connect(IConsumeObserver observer)
        {
            return _observers.Connect(observer);
        }

        public  Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            return Task.WhenAll(_pipes.Values.Select(x => x.Filter.Send(context, next)));
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, x => _pipes.Values.All(pipe => pipe.Filter.Inspect(x)));
        }

        public ConnectHandle Connect<T>(IMessageObserver<T> observer)
            where T : class
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            IMessageObserverConnector messagePipe = GetPipe<T, IMessageObserverConnector>();

            return messagePipe.Connect(observer);
        }

        public ConnectHandle Connect<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            IRequestFilterConnector<T> messagePipe = GetPipe<T, IRequestFilterConnector<T>>();

            return messagePipe.Connect(requestId, pipe);
        }

        TResult GetPipe<T, TResult>()
            where T : class
            where TResult : class
        {
            return _pipes.GetOrAdd(typeof(T), x =>
            {
                var messageConsumeFilter = new MessageConsumeFilter<T>();

                return new MessageFilter<T>(messageConsumeFilter, _observers);
            }).As<TResult>();
        }


        interface IMessageFilter
        {
            IFilter<ConsumeContext> Filter { get; }

            TResult As<TResult>()
                where TResult : class;
        }


        class MessageFilter<T> :
            IMessageFilter,
            IMessageObserver<T>
            where T : class
        {
            readonly MessageConsumeFilter<T> _filter;
            readonly IConsumeObserver _observer;
            ConnectHandle _filterHandle;

            public MessageFilter(MessageConsumeFilter<T> filter, IConsumeObserver observer)
            {
                _filter = filter;
                _observer = observer;

                // we subscribe to any events so that they are pushed up the stack
                _filterHandle = ((IMessageObserverConnector)filter).Connect(this);
            }

            public IFilter<ConsumeContext> Filter
            {
                get { return _filter; }
            }

            public TResult As<TResult>()
                where TResult : class
            {
                return _filter as TResult;
            }

            public Task PreDispatch(ConsumeContext<T> context)
            {
                return _observer.PreDispatch(context);
            }

            public Task PostDispatch(ConsumeContext<T> context)
            {
                return _observer.PostDispatch(context);
            }

            public Task DispatchFault(ConsumeContext<T> context, Exception exception)
            {
                return _observer.DispatchFault(context, exception);
            }
        }
    }
}