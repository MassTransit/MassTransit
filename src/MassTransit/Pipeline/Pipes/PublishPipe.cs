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
namespace MassTransit.Pipeline.Pipes
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using Observables;
    using PublishPipeSpecifications;
    using Util;


    public class PublishPipe :
        IPublishPipe
    {
        readonly PublishObservable _observers;
        readonly ConcurrentDictionary<Type, IMessagePipe> _outputPipes;
        readonly IPublishPipeSpecification _specification;

        public PublishPipe(IPublishPipeSpecification specification)
        {
            _specification = specification;
            _outputPipes = new ConcurrentDictionary<Type, IMessagePipe>();
            _observers = new PublishObservable();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("publishPipe");

            foreach (var outputPipe in _outputPipes.Values)
            {
                outputPipe.Probe(scope);
            }
        }

        ConnectHandle IPublishMessageObserverConnector.ConnectPublishMessageObserver<T>(IPublishMessageObserver<T> observer)
        {
            return GetPipe<T, IPublishMessageObserverConnector>().ConnectPublishMessageObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _observers.Connect(observer);
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        Task IPublishPipe.Send<T>(PublishContext<T> context)
        {
            return GetPipe<T>().Send(context);
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
            return _outputPipes.GetOrAdd(typeof(T), x => new MessagePipe<T>(_observers, _specification.GetMessageSpecification<T>()));
        }


        interface IMessagePipe :
            IPublishMessageObserverConnector,
            IProbeSite
        {
            Task Send<T>(PublishContext<T> context)
                where T : class;

            TResult As<TResult>()
                where TResult : class;
        }


        class MessagePipe<TMessage> :
            IMessagePipe
            where TMessage : class
        {
            readonly PublishObservable _observers;
            readonly Lazy<IMessagePublishPipe<TMessage>> _output;
            readonly IMessagePublishPipeSpecification<TMessage> _specification;

            public MessagePipe(PublishObservable observers, IMessagePublishPipeSpecification<TMessage> specification)

            {
                _output = new Lazy<IMessagePublishPipe<TMessage>>(CreateFilter);

                _observers = observers;
                _specification = specification;
            }

            TResult IMessagePipe.As<TResult>()
            {
                return _output.Value as TResult;
            }

            Task IMessagePipe.Send<T>(PublishContext<T> context)
            {
                var sendContext = context as PublishContext<TMessage>;
                if (sendContext == null)
                    throw new ArgumentException($"The argument type did not match the output type: {TypeMetadataCache<T>.ShortName}");

                return _output.Value.Send(sendContext);
            }

            public void Probe(ProbeContext context)
            {
                _output.Value.Probe(context);
            }

            ConnectHandle IPublishMessageObserverConnector.ConnectPublishMessageObserver<T>(IPublishMessageObserver<T> observer)
            {
                var connector = _output.Value as IMessagePublishPipe<T>;
                if (connector == null)
                    throw new ArgumentException($"The filter is not of the specified type: {typeof(T).Name}", nameof(observer));

                return connector.ConnectPublishMessageObserver(observer);
            }

            IMessagePublishPipe<TMessage> CreateFilter()
            {
                IPipe<PublishContext<TMessage>> messagePipe = _specification.BuildMessagePipe();

                IMessagePublishPipe<TMessage> messagePublishPipe = new MessagePublishPipe<TMessage>(messagePipe);

                var adapter = new PublishMessageObserverAdapter<TMessage>(_observers);

                messagePublishPipe.ConnectPublishMessageObserver(adapter);

                return messagePublishPipe;
            }
        }
    }
}