namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public interface IEventHubProducerProvider
    {
        Task<IEventHubProducer> GetProducer(Uri address);
    }
}
