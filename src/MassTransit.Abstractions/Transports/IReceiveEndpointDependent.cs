namespace MassTransit.Transports
{
    using System.Threading.Tasks;


    public interface IReceiveEndpointDependent
    {
        /// <summary>
        /// The task which is completed once the receive endpoint is completed
        /// </summary>
        Task Completed { get; }
    }
}
