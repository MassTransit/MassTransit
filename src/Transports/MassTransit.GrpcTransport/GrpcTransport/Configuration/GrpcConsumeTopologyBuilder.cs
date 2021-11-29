namespace MassTransit.GrpcTransport.Configuration
{
    using Contracts;
    using Fabric;


    public class GrpcConsumeTopologyBuilder :
        IGrpcConsumeTopologyBuilder
    {
        readonly NodeContext _context;
        readonly IMessageFabric _fabric;

        public GrpcConsumeTopologyBuilder(NodeContext context, IMessageFabric fabric)
        {
            _context = context;
            _fabric = fabric;
        }

        public string Exchange { get; set; }
        public string Queue { get; set; }

        public void ExchangeBind(string source, string destination, string routingKey)
        {
            _fabric.ExchangeBind(_context, source, destination, routingKey);
        }

        public void QueueBind(string source, string destination)
        {
            _fabric.QueueBind(_context, source, destination);
        }

        public void ExchangeDeclare(string name, ExchangeType exchangeType)
        {
            _fabric.ExchangeDeclare(_context, name, exchangeType);
        }

        public void QueueDeclare(string name)
        {
            _fabric.QueueDeclare(_context, name);
        }
    }
}
