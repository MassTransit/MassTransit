namespace MassTransit.WebJobs.EventHubsIntegration.Contexts
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Azure.EventHubs;


    public interface EventDataSendEndpointContext :
        PipeContext
    {
        /// <summary>
        /// The path of the messaging entity
        /// </summary>
        string EntityPath { get; }

        /// <summary>
        /// Send the message to the messaging entity
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task Send(EventData message);
    }
}