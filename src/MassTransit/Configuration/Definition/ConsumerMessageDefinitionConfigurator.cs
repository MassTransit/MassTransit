namespace MassTransit.Definition
{
    using System;
    using System.Linq.Expressions;
    using System.Text;


    public class ConsumerMessageDefinitionConfigurator<TConsumer, TMessage> :
        IConsumerMessageDefinitionConfigurator<TConsumer, TMessage>
        where TConsumer : class, IConsumer<TMessage>
        where TMessage : class
    {
        public void Publishes<T>()
            where T : class
        {
            throw new NotImplementedException();
        }

        public void Sends<T>()
            where T : class
        {
            throw new NotImplementedException();
        }

        public void Facet<T>(Expression<Func<TMessage, T>> propertyExpression,
            Action<IConsumerMessagePropertyDefinitionConfigurator<TConsumer, TMessage, T>> configure = null)
        {
            throw new NotImplementedException();
        }

        public void Property<T>(Expression<Func<TMessage, T>> propertyExpression,
            Action<IConsumerMessagePropertyDefinitionConfigurator<TConsumer, TMessage, T>> configure = null)
        {
            throw new NotImplementedException();
        }

        public void PartitionBy(Expression<Func<TMessage, Guid>> propertyExpression)
        {
            throw new NotImplementedException();
        }

        public void PartitionBy(Expression<Func<TMessage, string>> propertyExpression, Encoding encoding = default)
        {
            throw new NotImplementedException();
        }

        public void Resource(Expression<Func<TMessage, Uri>> propertyExpression,
            Action<IConsumerMessagePropertyDefinitionConfigurator<TConsumer, TMessage, Uri>> configure = null)
        {
            throw new NotImplementedException();
        }
    }
}
