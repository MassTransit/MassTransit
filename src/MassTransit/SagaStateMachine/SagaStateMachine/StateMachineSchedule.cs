namespace MassTransit
{
    using System;
    using System.Linq.Expressions;
    using Internals;


    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        public class StateMachineSchedule<TMessage> :
            Schedule<TInstance, TMessage>
            where TMessage : class
        {
            readonly string _name;
            readonly IReadProperty<TInstance, Guid?> _read;
            readonly ScheduleSettings<TInstance, TMessage> _settings;
            readonly IWriteProperty<TInstance, Guid?> _write;

            public StateMachineSchedule(string name, Expression<Func<TInstance, Guid?>> tokenIdExpression, ScheduleSettings<TInstance, TMessage> settings)
            {
                _name = name;
                _settings = settings;

                var propertyInfo = tokenIdExpression.GetPropertyInfo();

                _read = ReadPropertyCache<TInstance>.GetProperty<Guid?>(propertyInfo);
                _write = WritePropertyCache<TInstance>.GetProperty<Guid?>(propertyInfo);
            }

            string Schedule<TInstance>.Name => _name;
            public Event<TMessage> Received { get; set; }
            public Event<TMessage> AnyReceived { get; set; }

            public TimeSpan GetDelay(BehaviorContext<TInstance> context)
            {
                return _settings.DelayProvider(context);
            }

            public Guid? GetTokenId(TInstance instance)
            {
                return _read.Get(instance);
            }

            public void SetTokenId(TInstance instance, Guid? tokenId)
            {
                _write.Set(instance, tokenId);
            }
        }
    }
}
