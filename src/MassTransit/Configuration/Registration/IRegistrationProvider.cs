namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;


    public interface IRegistrationProvider
    {
        IConsumerRegistration GetConsumerRegistration(Type consumerType);

        IConsumerRegistration<T> GetConsumerRegistration<T>()
            where T : class, IConsumer;

        IEnumerable<IConsumerRegistration> GetConsumerRegistrations();
    }
}
