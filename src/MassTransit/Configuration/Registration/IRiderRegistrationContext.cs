namespace MassTransit.Registration
{
    using System.Collections.Generic;
    using Riders;


    public interface IRiderRegistrationContext :
        IRegistration
    {
        IEnumerable<T> GetRegistrations<T>();

        void UseHealthCheck(IRiderFactoryConfigurator configurator);
    }
}
