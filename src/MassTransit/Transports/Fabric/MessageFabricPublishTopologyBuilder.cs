namespace MassTransit.Transports.Fabric
{
    using Configuration;


    public class MessageFabricPublishTopologyBuilder<TContext, T> :
        IMessageFabricPublishTopologyBuilder
        where TContext : class
        where T : class
    {
        readonly TContext _context;
        readonly IMessageFabric<TContext, T> _messageFabric;

        public MessageFabricPublishTopologyBuilder(TContext context, IMessageFabric<TContext, T> messageFabric)
        {
            _context = context;
            _messageFabric = messageFabric;
        }

        public string ExchangeName { get; set; }
        public ExchangeType ExchangeType { get; set; }

        public IMessageFabricPublishTopologyBuilder CreateImplementedBuilder()
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
            IMessageFabricPublishTopologyBuilder
        {
            readonly IMessageFabricPublishTopologyBuilder _builder;
            string _exchangeName;

            public ImplementedBuilder(IMessageFabricPublishTopologyBuilder builder)
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
                        _builder.ExchangeBind(_builder.ExchangeName, _exchangeName, _builder.ExchangeType == ExchangeType.Topic ? "#" : default);
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

            public IMessageFabricPublishTopologyBuilder CreateImplementedBuilder()
            {
                return new ImplementedBuilder(this);
            }
        }
    }
}
