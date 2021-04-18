namespace MassTransit.GrpcTransport.Builders
{
    using Contexts;
    using Fabric;


    public class GrpcConsumeTopologyBuilder :
        IGrpcConsumeTopologyBuilder
    {
        readonly NodeContext _context;
        readonly IMessageFabric _messageFabric;

        public GrpcConsumeTopologyBuilder(NodeContext context, IMessageFabric messageFabric)
        {
            _context = context;
            _messageFabric = messageFabric;
        }

        public string Exchange { get; set; }
        public string Queue { get; set; }

        public void ExchangeBind(string source, string destination)
        {
            _messageFabric.ExchangeBind(_context, source, destination);
        }

        public void QueueBind(string source, string destination)
        {
            _messageFabric.QueueBind(_context, source, destination);
        }

        public void ExchangeDeclare(string name)
        {
            _messageFabric.ExchangeDeclare(_context, name);
        }

        public void QueueDeclare(string name, int concurrencyLimit)
        {
            _messageFabric.QueueDeclare(_context, name, concurrencyLimit);
        }
    }
}