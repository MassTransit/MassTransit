namespace MassTransit
{
    public interface IReceiveEndpointDependencyConnector
    {
        /// <summary>
        /// Add the observable receive endpoint as a dependent
        /// </summary>
        /// <param name="dependent"></param>
        void AddDependency(IReceiveEndpointDependentConnector dependent);
    }
}
