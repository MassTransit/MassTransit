namespace MassTransit.Registration
{
    using System.Collections.Generic;


    public interface IRiderRegistrationContext :
        IRegistration
    {
        IEnumerable<T> GetRegistrations<T>();
    }
}
