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
namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Pipeline;


    public class SendActivity<TInstance, TMessage> :
        Activity<TInstance>
        where TInstance : SagaStateMachineInstance
        where TMessage : class
    {
        readonly Func<TInstance, Uri> _destinationAddressFactory;
        readonly EventMessageFactory<TInstance, TMessage> _messageFactory;
        readonly IPipe<SendContext<TMessage>> _sendPipe;

        public SendActivity(Uri destinationAddress, EventMessageFactory<TInstance, TMessage> messageFactory)
        {
            _destinationAddressFactory = _ =>destinationAddress;
            _messageFactory = messageFactory;

            _sendPipe = Pipe.Empty<SendContext<TMessage>>();
        }

        public SendActivity(Uri destinationAddress, EventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback)
        {
            _destinationAddressFactory = _ => destinationAddress;
            _messageFactory = messageFactory;

            _sendPipe = Pipe.Execute(contextCallback);
        }

        public SendActivity(Func<TInstance,Uri> destinationAddressFactory, EventMessageFactory<TInstance, TMessage> messageFactory)
        {
            _destinationAddressFactory = destinationAddressFactory;
            _messageFactory = messageFactory;

            _sendPipe = Pipe.Empty<SendContext<TMessage>>();
        }

        public SendActivity(Func<TInstance, Uri> destinationAddressFactory, EventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback)
        {
            _destinationAddressFactory = destinationAddressFactory;
            _messageFactory = messageFactory;

            _sendPipe = Pipe.Execute(contextCallback);
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next)
        {
            return next.Faulted(context);
        }

        async Task Execute(BehaviorContext<TInstance> context)
        {
            ConsumeEventContext<TInstance> consumeContext = context.CreateConsumeContext();

            var message = _messageFactory(consumeContext);

            var destinationAddress = _destinationAddressFactory(consumeContext.Instance);

            var endpoint = await consumeContext.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, _sendPipe).ConfigureAwait(false);
        }
    }


    public class SendActivity<TInstance, TData, TMessage> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly Func<TInstance, Uri> _destinationAddressFactory;
        readonly EventMessageFactory<TInstance, TData, TMessage> _messageFactory;
        readonly IPipe<SendContext<TMessage>> _sendPipe;

        public SendActivity(Uri destinationAddress, EventMessageFactory<TInstance, TData, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
        {
            _destinationAddressFactory = _ => destinationAddress;
            _messageFactory = messageFactory;

            _sendPipe = Pipe.Execute(contextCallback);
        }

        public SendActivity(Uri destinationAddress, EventMessageFactory<TInstance, TData, TMessage> messageFactory)
        {
            _destinationAddressFactory = _ => destinationAddress;
            _messageFactory = messageFactory;

            _sendPipe = Pipe.Empty<SendContext<TMessage>>();
        }

        public SendActivity(Func<TInstance, Uri> destinationAddressFactory, EventMessageFactory<TInstance, TData, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
        {
            _destinationAddressFactory = destinationAddressFactory;
            _messageFactory = messageFactory;

            _sendPipe = Pipe.Execute(contextCallback);
        }

        public SendActivity(Func<TInstance, Uri> destinationAddressFactory, EventMessageFactory<TInstance, TData, TMessage> messageFactory)
        {
            _destinationAddressFactory = destinationAddressFactory;
            _messageFactory = messageFactory;

            _sendPipe = Pipe.Empty<SendContext<TMessage>>();
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeEventContext<TInstance, TData> consumeContext = context.CreateConsumeContext();

            var message = _messageFactory(consumeContext);

            var destinationAddress = _destinationAddressFactory(consumeContext.Instance);

            var endpoint = await consumeContext.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, _sendPipe).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
            Behavior<TInstance, TData> next)
        {
            return next.Faulted(context);
        }
    }
}