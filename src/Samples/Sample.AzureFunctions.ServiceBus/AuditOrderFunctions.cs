namespace Sample.AzureFunctions.ServiceBus
{
    using System.Threading;
    using System.Threading.Tasks;
    using Consumers;
    using MassTransit.WebJobs.EventHubsIntegration;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.WebJobs;


    public class AuditOrderFunctions
    {
        const string AuditOrderEventHubName = "input-hub";
        readonly IEventReceiver _receiver;

        public AuditOrderFunctions(IEventReceiver receiver)
        {
            _receiver = receiver;
        }

        [FunctionName("AuditOrder")]
        public Task AuditOrderAsync([EventHubTrigger(AuditOrderEventHubName, Connection = "AzureWebJobsEventHub")]
            EventData message, CancellationToken cancellationToken)
        {
            return _receiver.HandleConsumer<AuditOrderConsumer>(AuditOrderEventHubName, message, cancellationToken);
        }
    }
}
