namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using MassTransit.Saga;
    using Registration;


    public class MessageSessionSagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        public virtual void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            configurator.MessageSessionRepository();
        }
    }
}
