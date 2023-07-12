namespace MassTransit
{
    using Transports;


    public interface IReceiveEndpointDependentConnector
    {
        /// <summary>
        /// Add the dependent to receive endpoint. Receive endpoint will be stopped when dependent is Completed
        /// </summary>
        /// <param name="dependent"></param>
        void AddDependent(IReceiveEndpointDependent dependent);
    }
}
