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
namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Context;
    using MassTransit.Pipeline;


    public class PublishActivity<TInstance, TMessage> :
        Activity<TInstance>
        where TInstance : SagaStateMachineInstance
        where TMessage : class
    {
        readonly Func<ConsumeEventContext<TInstance>, TMessage> _messageFactory;
        readonly IPipe<PublishContext<TMessage>> _publishPipe;

        public PublishActivity(Func<ConsumeEventContext<TInstance>, TMessage> messageFactory)
        {
            _messageFactory = messageFactory;

            _publishPipe = Pipe.Empty<PublishContext<TMessage>>();
        }

        public PublishActivity(Func<ConsumeEventContext<TInstance>, TMessage> messageFactory,
            Action<PublishContext<TMessage>> contextCallback)
        {
            _messageFactory = messageFactory;

            _publishPipe = Pipe.New<PublishContext<TMessage>>(x =>
            {
                x.Execute(contextCallback);
            });
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this);
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            ConsumeContext consumeContext;
            if (!context.TryGetPayload(out consumeContext))
                throw new ContextException("The consume context could not be retrieved.");

            var consumeEventContext = new ConsumeEventContextImpl<TInstance>(context, consumeContext);

            TMessage message = _messageFactory(consumeEventContext);

            await consumeContext.Publish(message, _publishPipe);
        }

        async Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            ConsumeContext consumeContext;
            if (!context.TryGetPayload(out consumeContext))
                throw new ContextException("The consume context could not be retrieved.");

            var consumeEventContext = new ConsumeEventContextImpl<TInstance>(context, consumeContext);

            TMessage message = _messageFactory(consumeEventContext);

            await consumeContext.Publish(message, _publishPipe);
        }
    }


    public class PublishActivity<TInstance, TData, TMessage> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly Func<ConsumeEventContext<TInstance, TData>, TMessage> _messageFactory;
        readonly IPipe<PublishContext<TMessage>> _publishPipe;

        public PublishActivity(Func<ConsumeEventContext<TInstance, TData>, TMessage> messageFactory,
            Action<PublishContext<TMessage>> contextCallback)
        {
            _messageFactory = messageFactory;

            _publishPipe = Pipe.New<PublishContext<TMessage>>(x =>
            {
                x.Execute(contextCallback);
            });
        }

        public PublishActivity(Func<ConsumeEventContext<TInstance, TData>, TMessage> messageFactory)
        {
            _messageFactory = messageFactory;

            _publishPipe = Pipe.Empty<PublishContext<TMessage>>();
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this);
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeContext<TData> consumeContext;
            if (!context.TryGetPayload(out consumeContext))
                throw new ContextException("The consume context could not be retrieved.");

            var consumeEventContext = new ConsumeEventContextImpl<TInstance, TData>(context, consumeContext);

            TMessage message = _messageFactory(consumeEventContext);

            await consumeContext.Publish(message, _publishPipe);
        }
    }
}