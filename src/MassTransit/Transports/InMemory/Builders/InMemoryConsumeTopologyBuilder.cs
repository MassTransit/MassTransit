namespace MassTransit.Transports.InMemory.Builders
{
    using Fabric;


    public class InMemoryConsumeTopologyBuilder :
        IInMemoryConsumeTopologyBuilder
    {
        readonly IMessageFabric _messageFabric;

        public InMemoryConsumeTopologyBuilder(IMessageFabric messageFabric)
        {
            _messageFabric = messageFabric;
        }

        public string Exchange { get; set; }
        public string Queue { get; set; }

        public void ExchangeBind(string source, string destination)
        {
            _messageFabric.ExchangeBind(source, destination);
        }

        public void QueueBind(string source, string destination)
        {
            _messageFabric.QueueBind(source, destination);
        }

        public void ExchangeDeclare(string name)
        {
            _messageFabric.ExchangeDeclare(name);
        }

        public void QueueDeclare(string name, int concurrencyLimit)
        {
            _messageFabric.QueueDeclare(name, concurrencyLimit);
        }
    }
}