namespace MassTransit.Definition
{
    using System;
    using System.Linq.Expressions;
    using System.Text;


    public interface IConsumerMessageDefinitionConfigurator<TConsumer, TMessage>
        where TConsumer : class, IConsumer
        where TMessage : class
    {
        /// <summary>
        /// Defines a message type which may be published while consuming the message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        void Publishes<T>()
            where T : class;

        /// <summary>
        /// Defines a message type which may be sent while consuming the message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        void Sends<T>()
            where T : class;

        /// <summary>
        /// Defines a message property by which availability and success/failure can be distinguished. Dimension might be a better term.
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        void Facet<T>(Expression<Func<TMessage, T>> propertyExpression, Action<IConsumerMessagePropertyDefinitionConfigurator<TConsumer, TMessage, T>>
            configure = null);

        void Property<T>(Expression<Func<TMessage, T>> propertyExpression, Action<IConsumerMessagePropertyDefinitionConfigurator<TConsumer, TMessage, T>>
            configure = null);

        /// <summary>
        /// Messages delivered to the consumer should be partitioned by the specified message property
        /// </summary>
        /// <param name="propertyExpression"></param>
        void PartitionBy(Expression<Func<TMessage, Guid>> propertyExpression);

        /// <summary>
        /// Messages delivered to the consumer should be partitioned by the specified message property
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <param name="encoding">The text encoding to use for the string</param>
        void PartitionBy(Expression<Func<TMessage, string>> propertyExpression, Encoding encoding = default);

        /// <summary>
        /// Defines a message property as a resource, which may have availability concerns tracked by an external entity
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <param name="configure"></param>
        void Resource(Expression<Func<TMessage, Uri>> propertyExpression, Action<IConsumerMessagePropertyDefinitionConfigurator<TConsumer, TMessage, Uri>>
            configure = null);
    }
}
