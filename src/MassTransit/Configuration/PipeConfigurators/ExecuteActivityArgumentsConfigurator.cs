namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using Courier;
    using GreenPipes;


    public class ExecuteActivityArgumentsConfigurator<TActivity, TArguments> :
        IExecuteActivityArgumentsConfigurator<TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> _configurator;

        public ExecuteActivityArgumentsConfigurator(IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator)
        {
            _configurator = configurator;
        }

        public void AddPipeSpecification(IPipeSpecification<ExecuteActivityContext<TArguments>> specification)
        {
            _configurator.AddPipeSpecification(new ConsumeContextSpecificationProxy(specification));
        }


        class ConsumeContextSpecificationProxy :
            IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>
        {
            readonly IPipeSpecification<ExecuteActivityContext<TArguments>> _specification;

            public ConsumeContextSpecificationProxy(IPipeSpecification<ExecuteActivityContext<TArguments>> specification)
            {
                _specification = specification;
            }

            public void Apply(IPipeBuilder<ExecuteActivityContext<TActivity, TArguments>> builder)
            {
                var messageBuilder = builder as IPipeBuilder<ExecuteActivityContext<TArguments>>;

                if (messageBuilder != null)
                    _specification.Apply(messageBuilder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }
        }
    }
}
