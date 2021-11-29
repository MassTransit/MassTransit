namespace MassTransit.DependencyInjection.Registration
{
    public class RequestConsumerFutureEndpointDefinition<TFuture> :
        IEndpointDefinition<TFuture>
        where TFuture : class
    {
        readonly IConsumerDefinition _consumerDefinition;
        readonly IDefinition _definition;
        string _endpointName;

        public RequestConsumerFutureEndpointDefinition(IDefinition definition, IConsumerDefinition consumerDefinition)
        {
            _definition = definition;
            _consumerDefinition = consumerDefinition;
        }

        public bool ConfigureConsumeTopology => true;

        public void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
        }

        public bool IsTemporary => false;

        public int? PrefetchCount => default;

        public int? ConcurrentMessageLimit => _definition.ConcurrentMessageLimit;

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            string GetSeparator()
            {
                return formatter switch
                {
                    SnakeCaseEndpointNameFormatter f => f.Separator,
                    _ => ""
                };
            }

            var consumerEndpointName = _consumerDefinition.GetEndpointName(formatter);

            return _endpointName ??= formatter.SanitizeName(consumerEndpointName + GetSeparator() + "Future");
        }
    }
}
