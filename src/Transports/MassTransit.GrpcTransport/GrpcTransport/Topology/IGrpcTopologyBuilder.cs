namespace MassTransit.GrpcTransport.Topology
{
    using Contracts;


    public interface IGrpcTopologyBuilder
    {
        void ExchangeBind(string source, string destination, string routingKey);

        void QueueBind(string source, string destination);

        void ExchangeDeclare(string name, ExchangeType exchangeType);

        void QueueDeclare(string name);
    }
}
