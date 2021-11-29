namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    public class ConfiguratorPipeConnectorSpecification<TContext> :
        IPipeConfigurator<TContext>,
        IPipeConnectorSpecification
        where TContext : class, PipeContext
    {
        readonly IBuildPipeConfigurator<TContext> _configurator;

        public ConfiguratorPipeConnectorSpecification()
        {
            _configurator = new PipeConfigurator<TContext>();
        }

        public void AddPipeSpecification(IPipeSpecification<TContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        public void Connect(IPipeConnector connector)
        {
            IPipe<TContext> pipe = _configurator.Build();

            connector.ConnectPipe(pipe);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _configurator.Validate();
        }
    }
}
