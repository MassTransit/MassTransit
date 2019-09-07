namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using Courier;
    using GreenPipes;


    public class CompensateActivityLogConfigurator<TActivity, TLog> :
        ICompensateActivityLogConfigurator<TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> _configurator;

        public CompensateActivityLogConfigurator(IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> configurator)
        {
            _configurator = configurator;
        }

        public void AddPipeSpecification(IPipeSpecification<CompensateActivityContext<TLog>> specification)
        {
            _configurator.AddPipeSpecification(new ConsumeContextSpecificationProxy(specification));
        }


        class ConsumeContextSpecificationProxy :
            IPipeSpecification<CompensateActivityContext<TActivity, TLog>>
        {
            readonly IPipeSpecification<CompensateActivityContext<TLog>> _specification;

            public ConsumeContextSpecificationProxy(IPipeSpecification<CompensateActivityContext<TLog>> specification)
            {
                _specification = specification;
            }

            public void Apply(IPipeBuilder<CompensateActivityContext<TActivity, TLog>> builder)
            {
                var messageBuilder = builder as IPipeBuilder<CompensateActivityContext<TLog>>;

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
