namespace MassTransit.GrpcTransport.Topology.Builders
{
    public interface IGrpcTopologyBuilder
    {
        void ExchangeBind(string source, string destination);

        void QueueBind(string source, string destination);

        void ExchangeDeclare(string name);

        void QueueDeclare(string name, int concurrencyLimit);
    }
}
