namespace MassTransit
{
    using Middleware;


    public interface IPipeConnectorSpecification :
        ISpecification
    {
        void Connect(IPipeConnector connector);
    }
}
