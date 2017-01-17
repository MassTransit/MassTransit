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
    using SendPipeSpecifications;
    using Util;


    public class SendPipe :
        ISendPipe
    {
        public static readonly ISendPipe Empty = new SendPipe(new SendPipeSpecification());
        readonly SendObservable _observers;
        readonly ConcurrentDictionary<Type, IMessagePipe> _outputPipes;
        readonly ISendPipeSpecification _specification;

        public SendPipe(ISendPipeSpecification specification)
        {
            _specification = specification;
            _outputPipes = new ConcurrentDictionary<Type, IMessagePipe>();
            _observers = new SendObservable();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sendPipe");

            foreach (var outputPipe in _outputPipes.Values)
            {
                outputPipe.Probe(scope);
            }
        }

        ConnectHandle ISendMessageObserverConnector.ConnectSendMessageObserver<T>(ISendMessageObserver<T> observer)
        {
            return GetPipe<T, ISendMessageObserverConnector>().ConnectSendMessageObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        Task ISendPipe.Send<T>(SendContext<T> context)
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
            ISendMessageObserverConnector,
            IProbeSite
        {
            Task Send<T>(SendContext<T> context)
                where T : class;

            TResult As<TResult>()
                where TResult : class;
        }


        class MessagePipe<TMessage> :
            IMessagePipe
            where TMessage : class
        {
            readonly SendObservable _observers;
            readonly Lazy<IMessageSendPipe<TMessage>> _output;
            readonly IMessageSendPipeSpecification<TMessage> _specification;

            public MessagePipe(SendObservable observers, IMessageSendPipeSpecification<TMessage> specification)

            {
                _output = new Lazy<IMessageSendPipe<TMessage>>(CreateFilter);

                _observers = observers;
                _specification = specification;
            }

            TResult IMessagePipe.As<TResult>()
            {
                return _output.Value as TResult;
            }

            Task IMessagePipe.Send<T>(SendContext<T> context)
            {
                var sendContext = context as SendContext<TMessage>;
                if (sendContext == null)
                    throw new ArgumentException($"The argument type did not match the output type: {TypeMetadataCache<T>.ShortName}");

                return _output.Value.Send(sendContext);
            }

            public void Probe(ProbeContext context)
            {
                _output.Value.Probe(context);
            }

            ConnectHandle ISendMessageObserverConnector.ConnectSendMessageObserver<T>(ISendMessageObserver<T> observer)
            {
                var connector = _output.Value as IMessageSendPipe<T>;
                if (connector == null)
                    throw new ArgumentException($"The filter is not of the specified type: {typeof(T).Name}", nameof(observer));

                return connector.ConnectSendMessageObserver(observer);
            }

            IMessageSendPipe<TMessage> CreateFilter()
            {
                IPipe<SendContext<TMessage>> messagePipe = _specification.BuildMessagePipe();

                IMessageSendPipe<TMessage> messageSendPipe = new MessageSendPipe<TMessage>(messagePipe);

                var adapter = new SendMessageObserverAdapter<TMessage>(_observers);

                messageSendPipe.ConnectSendMessageObserver(adapter);

                return messageSendPipe;
            }
        }
    }
}