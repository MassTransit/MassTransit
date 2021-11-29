namespace MassTransit.InMemoryTransport.Configuration
{
    public interface IInMemoryTopologyBuilder
    {
        void ExchangeBind(string source, string destination);

        void QueueBind(string source, string destination);

        void ExchangeDeclare(string name);

        void QueueDeclare(string name, int concurrencyLimit);
    }
}
