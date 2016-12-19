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
    using GreenPipes;
    using MassTransit;


    public class PublishIfActivity<TInstance, TData, TMessage> : 
        Activity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly ConditionFactory<TInstance, TData> _conditionFactory;
        readonly Activity<TInstance, TData> _publishActivity;

        public PublishIfActivity(EventMessageFactory<TInstance, TData, TMessage> messageFactory, 
            ConditionFactory<TInstance, TData> conditionFactory)
        {
            _conditionFactory = conditionFactory;
            _publishActivity = new PublishActivity<TInstance, TData, TMessage>(messageFactory);
        }

        public PublishIfActivity(EventMessageFactory<TInstance, TData, TMessage> messageFactory,
            ConditionFactory<TInstance, TData> conditionFactory,
            Action<PublishContext<TMessage>> contextCallback)
        {
            _conditionFactory = conditionFactory;
            _publishActivity = new PublishActivity<TInstance, TData, TMessage>(messageFactory, contextCallback);
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            return _conditionFactory(context) ? _publishActivity.Execute(context, next) : next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, 
            Behavior<TInstance, TData> next)
            where TException : Exception
        {
            return _publishActivity.Faulted(context, next);
        }

        public void Probe(ProbeContext context)
        {
            _publishActivity.Probe(context);
        }
    }
}