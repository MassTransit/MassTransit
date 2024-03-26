namespace MassTransit.Configuration
{
    using System;


    public static class ActivityPipeConfiguratorExtensions
    {
        public static void AddPipeSpecification<TActivity, TArguments>(this IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator,
            IPipeSpecification<ExecuteActivityContext<TArguments>> specification)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>> filterSpecification =
                new PipeConfigurator<ExecuteActivityContext<TActivity, TArguments>>.SplitFilterPipeSpecification<ExecuteActivityContext<TArguments>>(
                    specification, (input, context) => input, context => context);

            configurator.AddPipeSpecification(filterSpecification);
        }

        public static void AddPipeSpecification<TActivity, TLog>(this IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> configurator,
            IPipeSpecification<CompensateActivityContext<TLog>> specification)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            IPipeSpecification<CompensateActivityContext<TActivity, TLog>> filterSpecification =
                new PipeConfigurator<CompensateActivityContext<TActivity, TLog>>.SplitFilterPipeSpecification<CompensateActivityContext<TLog>>(specification,
                    (input, context) => input, context => context);

            configurator.AddPipeSpecification(filterSpecification);
        }
    }
}
