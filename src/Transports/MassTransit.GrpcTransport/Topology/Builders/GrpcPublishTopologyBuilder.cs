namespace MassTransit.GrpcTransport.Topology.Builders
{
    using Contexts;
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

        public IGrpcPublishTopologyBuilder CreateImplementedBuilder()
        {
            return new ImplementedBuilder(this);
        }

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
                        _builder.ExchangeBind(_builder.ExchangeName, _exchangeName);
                }
            }

            public void ExchangeBind(string source, string destination)
            {
                _builder.ExchangeBind(source, destination);
            }

            public void QueueBind(string source, string destination)
            {
                _builder.QueueBind(source, destination);
            }

            public void ExchangeDeclare(string name)
            {
                _builder.ExchangeDeclare(name);
            }

            public void QueueDeclare(string name, int concurrencyLimit)
            {
                _builder.QueueDeclare(name, concurrencyLimit);
            }

            public IGrpcPublishTopologyBuilder CreateImplementedBuilder()
            {
                return new ImplementedBuilder(this);
            }
        }
    }
}