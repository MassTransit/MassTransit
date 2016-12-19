using System;
using System.Threading.Tasks;
using MassTransit;

namespace Automatonymous.Activities
{
    using GreenPipes;

    public class SendIfActivity<TInstance, TData, TMessage> : 
        Activity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly ConditionFactory<TInstance, TData> _conditionFactory;
        readonly Activity<TInstance, TData> _sendActivity;

        public SendIfActivity(EventMessageFactory<TInstance, TData, TMessage> messageFactory, 
            DestinationAddressProvider<TInstance, TData> destinationAddressProvider,
            ConditionFactory<TInstance, TData> conditionFactory)
        {
            _conditionFactory = conditionFactory;
            _sendActivity = new SendActivity<TInstance, TData, TMessage>(destinationAddressProvider, messageFactory);
        }

        public SendIfActivity(EventMessageFactory<TInstance, TData, TMessage> messageFactory,
            DestinationAddressProvider<TInstance, TData> destinationAddressProvider,
            ConditionFactory<TInstance, TData> conditionFactory,
            Action<SendContext<TMessage>> contextCallback)
        {
            _sendActivity = new SendActivity<TInstance, TData, TMessage>(destinationAddressProvider, messageFactory, contextCallback);
            _conditionFactory = conditionFactory;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            return _conditionFactory(context) ? _sendActivity.Execute(context, next) : next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, 
            Behavior<TInstance, TData> next)
            where TException : Exception
        {
            return _sendActivity.Faulted(context, next);
        }

        public void Probe(ProbeContext context)
        {
            _sendActivity.Probe(context);
        }
    }
}