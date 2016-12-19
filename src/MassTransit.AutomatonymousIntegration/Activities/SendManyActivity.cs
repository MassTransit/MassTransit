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
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;


    public class SendManyActivity<TInstance, TData, TInput, TMessage> : 
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class 
        where TMessage : class
    {
        readonly InputFactory<TInstance, TData, TInput> _inputFactory;
        readonly DestinationAddressProvider<TInstance, TData, TInput> _destinationAddressProvider;
        readonly EventMessageFactory<TInstance, TData, TInput, TMessage> _messageFactory;
        readonly IPipe<SendContext<TMessage>> _sendPipe;

        public SendManyActivity(
            InputFactory<TInstance, TData, TInput> inputFactory,
            DestinationAddressProvider<TInstance, TData, TInput> destinationAddressProvider,
            EventMessageFactory<TInstance, TData, TInput, TMessage> messageFactory)
        {
            _inputFactory = inputFactory;
            _destinationAddressProvider = destinationAddressProvider;
            _messageFactory = messageFactory;

            _sendPipe = Pipe.Empty<SendContext<TMessage>>();
        }

        public SendManyActivity(
            InputFactory<TInstance, TData, TInput> inputFactory,
            DestinationAddressProvider<TInstance, TData, TInput> destinationAddressProvider,
            EventMessageFactory<TInstance, TData, TInput, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
        {
            _inputFactory = inputFactory;
            _destinationAddressProvider = destinationAddressProvider;
            _messageFactory = messageFactory;

            _sendPipe = Pipe.Execute(contextCallback);
        }
        
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeEventContext<TInstance, TData> consumeContext = context.CreateConsumeContext();

            var inputs = _inputFactory(context);

            var tasks = inputs.Select(input =>
            {
                var message = _messageFactory(consumeContext, input);

                Uri destinationAddress = _destinationAddressProvider(context, input);

                return consumeContext.Send(destinationAddress, message, _sendPipe);
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("send");
            _sendPipe.Probe(scope);
        }
    }
}