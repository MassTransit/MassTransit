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


    public class PublishManyActivity<TInstance, TData, TInput, TMessage> :
        Activity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly InputFactory<TInstance, TData, TInput> _inputFactory;
        readonly EventMessageFactory<TInstance, TData, TInput, TMessage> _messageFactory;
        readonly IPipe<PublishContext<TMessage>> _publishPipe;

        public PublishManyActivity(
            InputFactory<TInstance, TData, TInput> inputFactory, 
            EventMessageFactory<TInstance, TData, TInput, TMessage> messageFactory)
            : this(inputFactory, messageFactory, context => Pipe.Empty<PublishContext<TMessage>>())
        {
        }

        public PublishManyActivity(
            InputFactory<TInstance, TData, TInput> inputFactory,
            EventMessageFactory<TInstance, TData, TInput, TMessage> messageFactory,
            Action<PublishContext<TMessage>> contextCallback)
        {
            _inputFactory = inputFactory;
            _messageFactory = messageFactory;
            _publishPipe = Pipe.Execute(contextCallback);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("publish");
            _publishPipe.Probe(scope);
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeEventContext<TInstance, TData> consumeContext = context.CreateConsumeContext();

            var inputs = _inputFactory(context);

            var tasks = inputs.Select(input =>
            {
                var message = _messageFactory(consumeContext, input);
                return consumeContext.Publish(message, _publishPipe);
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
            Behavior<TInstance, TData> next)
        {
            return next.Faulted(context);
        }
    }
}
