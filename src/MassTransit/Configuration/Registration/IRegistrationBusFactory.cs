namespace MassTransit.Registration
{
    using System.Collections.Generic;


    public interface IRegistrationBusFactory<in TContainerContext>
        where TContainerContext : class
    {
        IBusInstance CreateBus(IRegistrationContext<TContainerContext> context, IEnumerable<IBusInstanceSpecification> specifications = null);
    }
}
