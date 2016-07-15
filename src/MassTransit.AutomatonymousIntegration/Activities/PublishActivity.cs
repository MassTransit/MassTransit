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
namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Pipeline;


    public class PublishActivity<TInstance, TMessage> :
        Activity<TInstance>
        where TInstance : SagaStateMachineInstance
        where TMessage : class
    {
        readonly EventMessageFactory<TInstance, TMessage> _messageFactory;
        readonly IPipe<PublishContext<TMessage>> _publishPipe;

        public PublishActivity(EventMessageFactory<TInstance, TMessage> messageFactory)
        {
            _messageFactory = messageFactory;

            _publishPipe = Pipe.Empty<PublishContext<TMessage>>();
        }

        public PublishActivity(EventMessageFactory<TInstance, TMessage> messageFactory,
            Action<PublishContext<TMessage>> contextCallback)
        {
            _messageFactory = messageFactory;

            _publishPipe = Pipe.Execute(contextCallback);
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

        Task Execute(BehaviorContext<TInstance> context)
        {
            var consumeContext = context.CreateConsumeContext();

            TMessage message = _messageFactory(consumeContext);

            return consumeContext.Publish(message, _publishPipe);
        }
    }


    public class PublishActivity<TInstance, TData, TMessage> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly EventMessageFactory<TInstance, TData, TMessage> _messageFactory;
        readonly IPipe<PublishContext<TMessage>> _publishPipe;

        public PublishActivity(EventMessageFactory<TInstance, TData, TMessage> messageFactory,
            Action<PublishContext<TMessage>> contextCallback)
        {
            _messageFactory = messageFactory;

            _publishPipe = Pipe.Execute(contextCallback);
        }

        public PublishActivity(EventMessageFactory<TInstance, TData, TMessage> messageFactory)
        {
            _messageFactory = messageFactory;

            _publishPipe = Pipe.Empty<PublishContext<TMessage>>();
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            var consumeContext = context.CreateConsumeContext();

            TMessage message = _messageFactory(consumeContext);

            await consumeContext.Publish(message, _publishPipe).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
            Behavior<TInstance, TData> next)
        {
            return next.Faulted(context);
        }
    }
}