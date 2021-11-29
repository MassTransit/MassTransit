namespace MassTransit.Transports
{
    using System.Threading.Tasks;


    public interface IReceiveEndpointDependency
    {
        /// <summary>
        /// The task which is completed once the receive endpoint is ready
        /// </summary>
        Task Ready { get; }
    }
}
