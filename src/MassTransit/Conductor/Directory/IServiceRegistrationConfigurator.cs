namespace MassTransit.Conductor.Directory
{
    using System;
    using System.Linq.Expressions;
    using System.Text;


    public interface IServiceRegistrationConfigurator<TInput>
    {
        void PartitionBy(Expression<Func<TInput, Guid>> propertyExpression);
        void PartitionBy(Expression<Func<TInput, string>> propertyExpression, Encoding encoding = default);
    }
}
