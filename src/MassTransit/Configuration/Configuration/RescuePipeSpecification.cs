namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    public class RescuePipeSpecification<TContext, TRescue> :
        ExceptionSpecification,
        IPipeSpecification<TContext>,
        IRescueConfigurator<TContext, TRescue>
        where TContext : class, PipeContext
        where TRescue : class, TContext
    {
        readonly IPipeConfigurator<TContext> _contextPipeConfigurator;
        readonly IBuildPipeConfigurator<TRescue> _pipeConfigurator;
        readonly RescueContextFactory<TContext, TRescue> _rescueContextFactory;

        public RescuePipeSpecification(RescueContextFactory<TContext, TRescue> rescueContextFactory)
        {
            _rescueContextFactory = rescueContextFactory;

            _pipeConfigurator = new PipeConfigurator<TRescue>();
            _contextPipeConfigurator = new ContextPipeConfigurator(_pipeConfigurator);
        }

        public void Apply(IPipeBuilder<TContext> builder)
        {
            IPipe<TRescue> rescuePipe = _pipeConfigurator.Build();

            builder.AddFilter(new RescueFilter<TContext, TRescue>(rescuePipe, Filter, _rescueContextFactory));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_rescueContextFactory == null)
                yield return this.Failure("RescueContextFactory", "must not be null");

            foreach (var result in _pipeConfigurator.Validate())
                yield return result;
        }

        IPipeConfigurator<TContext> IRescueConfigurator<TContext, TRescue>.ContextPipe => _contextPipeConfigurator;

        void IPipeConfigurator<TRescue>.AddPipeSpecification(IPipeSpecification<TRescue> specification)
        {
            _pipeConfigurator.AddPipeSpecification(specification);
        }


        class ContextPipeConfigurator :
            IPipeConfigurator<TContext>
        {
            readonly IPipeConfigurator<TRescue> _configurator;

            public ContextPipeConfigurator(IPipeConfigurator<TRescue> configurator)
            {
                _configurator = configurator;
            }

            public void AddPipeSpecification(IPipeSpecification<TContext> specification)
            {
                _configurator.AddPipeSpecification(new SplitFilterPipeSpecification<TRescue, TContext>(specification,
                    InputContext, Context));
            }

            static TRescue Context(TRescue context)
            {
                return context;
            }

            static TRescue InputContext(TRescue input, TContext context)
            {
                return input;
            }
        }
    }
}
