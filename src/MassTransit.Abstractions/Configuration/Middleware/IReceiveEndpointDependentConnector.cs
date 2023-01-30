namespace MassTransit
{
    public interface IReceiveEndpointDependentConnector :
        IReceiveEndpointObserverConnector
    {
        /// <summary>
        /// Add the observable receive endpoint as a dependency
        /// </summary>
        /// <param name="dependency"></param>
        void AddDependent(IReceiveEndpointObserverConnector dependency);
    }
}
