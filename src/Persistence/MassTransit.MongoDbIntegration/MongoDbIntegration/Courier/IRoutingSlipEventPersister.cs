namespace MassTransit.MongoDbIntegration.Courier
{
    using System;
    using System.Threading.Tasks;
    using Events;


    public interface IRoutingSlipEventPersister
    {
        Task Persist<T>(Guid trackingNumber, T @event)
            where T : RoutingSlipEventDocument;
    }
}
