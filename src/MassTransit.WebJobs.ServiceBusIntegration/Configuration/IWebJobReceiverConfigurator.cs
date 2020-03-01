namespace MassTransit.WebJobs.ServiceBusIntegration.Configuration
{
    using Azure.ServiceBus.Core.Configurators;


    public interface IWebJobReceiverConfigurator :
        IReceiverConfigurator
    {
    }
}
