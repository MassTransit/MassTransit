namespace MassTransit.Registration
{
    using System.Collections.Generic;


    public interface IRegistrationBusFactory
    {
        IBusInstance CreateBus(IRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications = null);
    }
}
