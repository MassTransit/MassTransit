namespace MassTransit.GrpcTransport.Topology.Builders
{
    using Contexts;
    using Contracts;
    using Fabric;


    public class GrpcPublishTopologyBuilder :
        IGrpcPublishTopologyBuilder
    {
        readonly NodeContext _context;
        readonly IMessageFabric _messageFabric;

        public GrpcPublishTopologyBuilder(NodeContext context, IMessageFabric messageFabric)
        {
            _context = context;
            _messageFabric = messageFabric;
        }

        public string ExchangeName { get; set; }
        public ExchangeType ExchangeType { get; set; }

        public IGrpcPublishTopologyBuilder CreateImplementedBuilder()
        {
            return new ImplementedBuilder(this);
        }

        public void ExchangeBind(string source, string destination, string routingKey)
        {
            _messageFabric.ExchangeBind(_context, source, destination, routingKey);
        }

        public void QueueBind(string source, string destination)
        {
            _messageFabric.QueueBind(_context, source, destination);
        }

        public void ExchangeDeclare(string name, ExchangeType exchangeType)
        {
            _messageFabric.ExchangeDeclare(_context, name, exchangeType);
        }

        public void QueueDeclare(string name)
        {
            _messageFabric.QueueDeclare(_context, name);
        }


        class ImplementedBuilder :
            IGrpcPublishTopologyBuilder
        {
            readonly IGrpcPublishTopologyBuilder _builder;
            string _exchangeName;

            public ImplementedBuilder(IGrpcPublishTopologyBuilder builder)
            {
                _builder = builder;
            }

            public string ExchangeName
            {
                get => _exchangeName;
                set
                {
                    _exchangeName = value;
                    if (_builder.ExchangeName != null)
                        _builder.ExchangeBind(_builder.ExchangeName, _exchangeName, default);
                }
            }

            public ExchangeType ExchangeType { get; set; }

            public void ExchangeBind(string source, string destination, string routingKey)
            {
                _builder.ExchangeBind(source, destination, routingKey);
            }

            public void QueueBind(string source, string destination)
            {
                _builder.QueueBind(source, destination);
            }

            public void ExchangeDeclare(string name, ExchangeType exchangeType)
            {
                _builder.ExchangeDeclare(name, exchangeType);
            }

            public void QueueDeclare(string name)
            {
                _builder.QueueDeclare(name);
            }

            public IGrpcPublishTopologyBuilder CreateImplementedBuilder()
            {
                return new ImplementedBuilder(this);
            }
        }
    }
}
