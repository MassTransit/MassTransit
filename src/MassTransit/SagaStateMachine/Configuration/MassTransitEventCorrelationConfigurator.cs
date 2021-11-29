namespace MassTransit.Configuration
{
    using System;
    using System.Linq.Expressions;
    using Middleware;
    using Saga;
    using SagaStateMachine;


    public class MassTransitEventCorrelationConfigurator<TInstance, TData> :
        IEventCorrelationConfigurator<TInstance, TData>,
        IEventCorrelationBuilder
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Event<TData> _event;
        readonly SagaStateMachine<TInstance> _machine;
        IFilter<ConsumeContext<TData>> _messageFilter;
        IPipe<ConsumeContext<TData>> _missingPipe;
        ISagaFactory<TInstance, TData> _sagaFactory;
        SagaFilterFactory<TInstance, TData> _sagaFilterFactory;

        public MassTransitEventCorrelationConfigurator(SagaStateMachine<TInstance> machine, Event<TData> @event, EventCorrelation existingCorrelation)
        {
            _event = @event;
            _machine = machine;

            InsertOnInitial = false;
            ReadOnly = false;
            ConfigureConsumeTopology = true;

            _sagaFactory = new DefaultSagaFactory<TInstance, TData>();

            var correlation = existingCorrelation as EventCorrelation<TInstance, TData>;
            if (correlation != null)
            {
                _sagaFilterFactory = correlation.FilterFactory;
                _messageFilter = correlation.MessageFilter;
            }
        }

        public EventCorrelation Build()
        {
            return new MessageEventCorrelation<TInstance, TData>(_machine, _event, _sagaFilterFactory, _messageFilter, _missingPipe, _sagaFactory,
                InsertOnInitial, ReadOnly, ConfigureConsumeTopology);
        }

        public bool InsertOnInitial { get; set; }

        public bool ReadOnly { get; set; }

        public bool ConfigureConsumeTopology { get; set; }

        public IEventCorrelationConfigurator<TInstance, TData> CorrelateById(Func<ConsumeContext<TData>, Guid> selector)
        {
            _messageFilter = new CorrelationIdMessageFilter<TData>(selector);

            _sagaFilterFactory = (repository, policy, sagaPipe) => new CorrelatedSagaFilter<TInstance, TData>(repository, policy, sagaPipe);

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> CorrelateById<T>(Expression<Func<TInstance, T>> propertyExpression,
            Func<ConsumeContext<TData>, T> selector)
            where T : struct
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            _sagaFilterFactory = (repository, policy, sagaPipe) =>
            {
                var propertySelector = new NotDefaultValueTypeSagaQueryPropertySelector<TData, T>(selector);
                var queryFactory = new PropertyExpressionSagaQueryFactory<TInstance, TData, T>(propertyExpression, propertySelector);

                return new QuerySagaFilter<TInstance, TData>(repository, policy, queryFactory, sagaPipe);
            };

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> CorrelateBy<T>(Expression<Func<TInstance, T?>> propertyExpression,
            Func<ConsumeContext<TData>, T?> selector)
            where T : struct
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            _sagaFilterFactory = (repository, policy, sagaPipe) =>
            {
                var propertySelector = new HasValueTypeSagaQueryPropertySelector<TData, T>(selector);
                var queryFactory = new PropertyExpressionSagaQueryFactory<TInstance, TData, T?>(propertyExpression, propertySelector);

                return new QuerySagaFilter<TInstance, TData>(repository, policy, queryFactory, sagaPipe);
            };

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> CorrelateBy<T>(Expression<Func<TInstance, T>> propertyExpression,
            Func<ConsumeContext<TData>, T> selector)
            where T : class
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            _sagaFilterFactory = (repository, policy, sagaPipe) =>
            {
                var propertySelector = new SagaQueryPropertySelector<TData, T>(selector);
                var queryFactory = new PropertyExpressionSagaQueryFactory<TInstance, TData, T>(propertyExpression, propertySelector);

                return new QuerySagaFilter<TInstance, TData>(repository, policy, queryFactory, sagaPipe);
            };

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> SelectId(Func<ConsumeContext<TData>, Guid> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            _messageFilter = new CorrelationIdMessageFilter<TData>(selector);

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> CorrelateBy(Expression<Func<TInstance, ConsumeContext<TData>, bool>> correlationExpression)
        {
            if (correlationExpression == null)
                throw new ArgumentNullException(nameof(correlationExpression));

            _sagaFilterFactory = (repository, policy, sagaPipe) =>
            {
                var queryFactory = new ExpressionCorrelationSagaQueryFactory<TInstance, TData>(correlationExpression);

                return new QuerySagaFilter<TInstance, TData>(repository, policy, queryFactory, sagaPipe);
            };

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> SetSagaFactory(SagaFactoryMethod<TInstance, TData> factoryMethod)
        {
            _sagaFactory = new FactoryMethodSagaFactory<TInstance, TData>(factoryMethod);

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> OnMissingInstance(
            Func<IMissingInstanceConfigurator<TInstance, TData>, IPipe<ConsumeContext<TData>>> getMissingPipe)
        {
            if (getMissingPipe == null)
                throw new ArgumentNullException(nameof(getMissingPipe));

            var configurator = new EventMissingInstanceConfigurator<TInstance, TData>();

            _missingPipe = getMissingPipe(configurator);

            return this;
        }
    }
}
