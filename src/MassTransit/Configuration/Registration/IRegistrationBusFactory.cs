namespace MassTransit.Registration
{
    using System.Collections.Generic;


    public interface IRegistrationBusFactory
    {
        IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications = null);
    }
}
