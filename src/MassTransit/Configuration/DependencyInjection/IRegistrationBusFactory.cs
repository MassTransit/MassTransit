namespace MassTransit
{
    using System.Collections.Generic;
    using Configuration;
    using Transports;


    public interface IRegistrationBusFactory
    {
        IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName);
    }
}
