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


    public class FaultedSendActivity<TInstance, TData, TException, TMessage> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TMessage : class
        where TException : Exception
    {
        readonly Uri _destionationAddress;
        readonly EventExceptionMessageFactory<TInstance, TData, TException, TMessage> _messageFactory;
        readonly IPipe<SendContext<TMessage>> _sendPipe;

        public FaultedSendActivity(Uri destionationAddress, EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
        {
            _destionationAddress = destionationAddress;
            _messageFactory = messageFactory;

            _sendPipe = Pipe.Execute(contextCallback);
        }

        public FaultedSendActivity(Uri destionationAddress, EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory)
        {
            _destionationAddress = destionationAddress;
            _messageFactory = messageFactory;

            _sendPipe = Pipe.Empty<SendContext<TMessage>>();
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance, TData>.Faulted<T>(BehaviorExceptionContext<TInstance, TData, T> context,
            Behavior<TInstance, TData> next)
        {
            ConsumeExceptionEventContext<TInstance, TData, TException> exceptionContext;
            if (context.TryGetExceptionContext(out exceptionContext))
            {
                var message = _messageFactory(exceptionContext);

                var endpoint = await exceptionContext.GetSendEndpoint(_destionationAddress).ConfigureAwait(false);

                await endpoint.Send(message, _sendPipe).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }
}