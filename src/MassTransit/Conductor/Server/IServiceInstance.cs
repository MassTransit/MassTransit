namespace MassTransit.Conductor.Server
{
    using System;
    using System.Threading.Tasks;


    public interface IServiceInstance
    {
        NewId InstanceId { get; }

        Guid EndpointId { get; }

        Uri InstanceAddress { get; }

        /// <summary>
        /// Configure the message type consumers on the instance endpoint, to support Link, Unlink, Up, Down, etc.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <typeparam name="T"></typeparam>
        void ConfigureMessageEndpoint<T>(IMessageEndpoint<T> endpoint)
            where T : class;

        Task NotifyUp<T>(IMessageEndpoint<T> endpoint)
            where T : class;

        Task NotifyDown<T>(IMessageEndpoint<T> endpoint)
            where T : class;
    }
}
