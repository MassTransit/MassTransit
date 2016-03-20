namespace MassTransit.HttpTransport
{
    using Hosting;
    using Transports;
    using Util;


    public interface IHttpHost : IBusHost
    {
        HttpHostSettings Settings { get; }

        IOwinHostCache OwinHostCache { get; }

        /// <summary>
        /// The supervisor for the host, which indicates when it's being stopped
        /// </summary>
        ITaskSupervisor Supervisor { get; }
    }
}