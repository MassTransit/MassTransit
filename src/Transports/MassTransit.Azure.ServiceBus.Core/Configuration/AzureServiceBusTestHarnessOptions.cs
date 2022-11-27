namespace MassTransit
{
    public class AzureServiceBusTestHarnessOptions
    {
        /// <summary>
        /// Remove all topics, queues, and subscriptions from the service bus namespace when starting the test harness (via a hosted service)
        /// </summary>
        public bool CleanNamespace { get; set; }
    }
}
