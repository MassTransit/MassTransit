namespace MassTransit.WebJobs.EventHubsIntegration.Configuration
{
    using System.Threading;
    using Azure.ServiceBus.Core.Configurators;


    public interface IWebJobReceiverConfigurator :
        IReceiverConfigurator
    {
        CancellationToken CancellationToken { set; }
    }
}
