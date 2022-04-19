namespace MassTransit
{
    using System.Collections.Generic;
    using Configuration;


    public interface IRiderRegistrationContext :
        IRegistrationContext
    {
        IEnumerable<T> GetRegistrations<T>()
            where T : class, IRegistration;
    }
}
