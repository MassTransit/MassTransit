namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Linq.Expressions;
    using Internals;


    public class StateMachineSchedule<TSaga, TMessage> :
        Schedule<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly string _name;
        readonly IReadProperty<TSaga, Guid?> _read;
        readonly ScheduleSettings<TSaga, TMessage> _settings;
        readonly IWriteProperty<TSaga, Guid?> _write;

        public StateMachineSchedule(string name, Expression<Func<TSaga, Guid?>> tokenIdExpression, ScheduleSettings<TSaga, TMessage> settings)
        {
            _name = name;
            _settings = settings;

            var propertyInfo = tokenIdExpression.GetPropertyInfo();

            _read = ReadPropertyCache<TSaga>.GetProperty<Guid?>(propertyInfo);
            _write = WritePropertyCache<TSaga>.GetProperty<Guid?>(propertyInfo);
        }

        string Schedule<TSaga>.Name => _name;
        public Event<TMessage> Received { get; set; }
        public Event<TMessage> AnyReceived { get; set; }

        public TimeSpan GetDelay(BehaviorContext<TSaga> context)
        {
            return _settings.DelayProvider(context);
        }

        public Guid? GetTokenId(TSaga instance)
        {
            return _read.Get(instance);
        }

        public void SetTokenId(TSaga instance, Guid? tokenId)
        {
            _write.Set(instance, tokenId);
        }
    }
}
