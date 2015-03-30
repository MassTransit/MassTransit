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
        IConsumePipeConnector,
        IRequestPipeConnector,
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector
    {
        readonly ConsumeObserverConnectable _observers;
        readonly ConcurrentDictionary<Type, IMessageFilter> _pipes;

        public MessageTypeConsumeFilter()
        {
            _pipes = new ConcurrentDictionary<Type, IMessageFilter>();
            _observers = new ConsumeObserverConnectable();
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            IConsumePipeConnector<T> messagePipe = GetPipe<T, IConsumePipeConnector<T>>();

            return messagePipe.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _observers.Connect(observer);
        }

        public  Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            return Task.WhenAll(_pipes.Values.Select(x => x.Filter.Send(context, next)));
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this, x => _pipes.Values.All(pipe => pipe.Filter.Visit(x)));
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            IConsumeMessageObserverConnector messagePipe = GetPipe<T, IConsumeMessageObserverConnector>();

            return messagePipe.ConnectConsumeMessageObserver(observer);
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
            IConsumeMessageObserver<T>
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
                _filterHandle = ((IConsumeMessageObserverConnector)filter).ConnectConsumeMessageObserver(this);
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

            public Task PreConsume(ConsumeContext<T> context)
            {
                return _observer.PreConsume(context);
            }

            public Task PostConsume(ConsumeContext<T> context)
            {
                return _observer.PostConsume(context);
            }

            public Task ConsumeFault(ConsumeContext<T> context, Exception exception)
            {
                return _observer.ConsumeFault(context, exception);
            }
        }
    }
}