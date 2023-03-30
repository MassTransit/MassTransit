namespace MassTransit
{
    using Transports;


    public interface IReceiveEndpointDependencyConnector
    {
        /// <summary>
        /// Add receive endpoint dependency. Endpoint will be started when dependency is Ready
        /// </summary>
        /// <param name="dependency"></param>
        void AddDependency(IReceiveEndpointDependency dependency);
    }
}
