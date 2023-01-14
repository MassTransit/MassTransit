namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public interface IEventHubProducerProvider :
        ISendObserverConnector
    {
        Task<IEventHubProducer> GetProducer(Uri address);
    }
}
