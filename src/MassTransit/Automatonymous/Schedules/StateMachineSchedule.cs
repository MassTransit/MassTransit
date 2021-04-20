namespace Automatonymous.Schedules
{
    using System;
    using System.Linq.Expressions;
    using GreenPipes.Internals.Reflection;
    using MassTransit.Internals.Extensions;


    public class StateMachineSchedule<TInstance, TMessage> :
        Schedule<TInstance, TMessage>
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly string _name;
        readonly ScheduleSettings<TInstance, TMessage> _settings;
        readonly ReadWriteProperty<TInstance, Guid?> _tokenIdProperty;

        public StateMachineSchedule(string name, Expression<Func<TInstance, Guid?>> tokenIdExpression, ScheduleSettings<TInstance, TMessage> settings)
        {
            _name = name;
            _settings = settings;

            _tokenIdProperty = new ReadWriteProperty<TInstance, Guid?>(tokenIdExpression.GetPropertyInfo());
        }

        string Schedule<TInstance>.Name => _name;
        public Event<TMessage> Received { get; set; }
        public Event<TMessage> AnyReceived { get; set; }

        public TimeSpan GetDelay(ConsumeEventContext<TInstance> context)
        {
            return _settings.DelayProvider(context);
        }

        public Guid? GetTokenId(TInstance instance)
        {
            return _tokenIdProperty.Get(instance);
        }

        public void SetTokenId(TInstance instance, Guid? tokenId)
        {
            _tokenIdProperty.Set(instance, tokenId);
        }
    }
}
