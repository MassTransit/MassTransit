namespace MassTransit.Conductor.Configuration
{
    using System;
    using System.Linq.Expressions;


    public interface IMessagePartitionConfigurator<TMessage>
        where TMessage : class
    {
        IMessagePartitionConfigurator<TMessage> Property<T>(Expression<Func<TMessage, T>> propertyExpression);
    }
}
