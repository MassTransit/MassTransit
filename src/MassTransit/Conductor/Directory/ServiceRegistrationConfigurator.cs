namespace MassTransit.Conductor.Directory
{
    using System;
    using System.Linq.Expressions;
    using System.Text;


    public class ServiceRegistrationConfigurator<TInput, TResult> :
        IServiceRegistrationConfigurator<TInput>
        where TResult : class
        where TInput : class
    {
        readonly IServiceProviderDefinition<TInput, TResult> _definition;
        readonly IServiceRegistration<TResult> _registration;

        public ServiceRegistrationConfigurator(IServiceRegistration<TResult> registration, IServiceProviderDefinition<TInput, TResult> definition)
        {
            _registration = registration;
            _definition = definition;
        }

        public void PartitionBy(Expression<Func<TInput, Guid>> propertyExpression)
        {
        }

        public void PartitionBy(Expression<Func<TInput, string>> propertyExpression, Encoding encoding = default)
        {
        }

        public void ConfigureProvider()
        {
            _registration.AddProvider(_definition.CreateProvider());
        }
    }
}
