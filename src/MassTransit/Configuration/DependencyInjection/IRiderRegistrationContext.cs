namespace MassTransit
{
    using System.Collections.Generic;


    public interface IRiderRegistrationContext :
        IRegistrationContext
    {
        IEnumerable<T> GetRegistrations<T>();
    }
}
