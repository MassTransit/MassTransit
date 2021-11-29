namespace MassTransit
{
    using System;
    using System.Linq.Expressions;


    public interface IEventCorrelationConfigurator<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
    {
        /// <summary>
        /// If set to true, the state machine suggests that the saga instance be inserted blinding prior to the get/lock
        /// using a weaker isolation level. This prevents range locks in the database from slowing inserts.
        /// </summary>
        bool InsertOnInitial { set; }

        /// <summary>
        /// If set to true, changes to the saga instance will not be saved to the repository. Note that the in-memory saga repository
        /// does not support read-only since the changes are made directly to the saga instance.
        /// </summary>
        bool ReadOnly { set; }

        /// <summary>
        /// If set to false, the event type will not be configured as part of the broker topology
        /// </summary>
        bool ConfigureConsumeTopology { set; }

        /// <summary>
        /// Correlate to the saga instance by CorrelationId, using the id from the event data
        /// </summary>
        /// <param name="selector">Returns the CorrelationId from the event data</param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TSaga, TMessage> CorrelateById(Func<ConsumeContext<TMessage>, Guid> selector);

        /// <summary>
        /// Correlate to the saga instance by a single value property, matched to the property value of the message
        /// </summary>
        /// <param name="propertyExpression">The instance property</param>
        /// <param name="selector">The identifier selector for the message</param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TSaga, TMessage> CorrelateById<T>(Expression<Func<TSaga, T>> propertyExpression,
            Func<ConsumeContext<TMessage>, T> selector)
            where T : struct;

        /// <summary>
        /// Correlate to the saga instance by a single property, matched to the property value of the message
        /// </summary>
        /// <param name="propertyExpression">The instance property</param>
        /// <param name="selector"></param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TSaga, TMessage> CorrelateBy<T>(Expression<Func<TSaga, T?>> propertyExpression,
            Func<ConsumeContext<TMessage>, T?> selector)
            where T : struct;

        /// <summary>
        /// Correlate to the saga instance by a single property, matched to the property value of the message
        /// </summary>
        /// <param name="propertyExpression">The instance property</param>
        /// <param name="selector"></param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TSaga, TMessage> CorrelateBy<T>(Expression<Func<TSaga, T>> propertyExpression, Func<ConsumeContext<TMessage>, T> selector)
            where T : class;

        /// <summary>
        /// When creating a new saga instance, initialize the saga CorrelationId with the id from the event data
        /// </summary>
        /// <param name="selector">Returns the CorrelationId from the event data</param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TSaga, TMessage> SelectId(Func<ConsumeContext<TMessage>, Guid> selector);

        /// <summary>
        /// Specify the correlation expression for the event
        /// </summary>
        /// <param name="correlationExpression"></param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TSaga, TMessage> CorrelateBy(Expression<Func<TSaga, ConsumeContext<TMessage>, bool>> correlationExpression);

        /// <summary>
        /// Creates a new instance of the saga, and if appropriate, pre-inserts the saga instance to the database. If the saga already exists, any
        /// exceptions from the insert are suppressed and processing continues normally.
        /// </summary>
        /// <param name="factoryMethod">The factory method for the saga</param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TSaga, TMessage> SetSagaFactory(SagaFactoryMethod<TSaga, TMessage> factoryMethod);

        /// <summary>
        /// If an event is consumed that is not matched to an existing saga instance, discard the event without throwing an exception.
        /// The default behavior is to throw an exception, which moves the event into the error queue for later processing
        /// </summary>
        /// <param name="getBehavior">The configuration call to specify the behavior on missing instance</param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TSaga, TMessage> OnMissingInstance(Func<IMissingInstanceConfigurator<TSaga, TMessage>,
            IPipe<ConsumeContext<TMessage>>> getBehavior);
    }
}
