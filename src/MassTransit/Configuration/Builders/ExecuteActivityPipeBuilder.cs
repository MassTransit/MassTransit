namespace MassTransit.Builders
{
    using System;
    using System.Collections.Generic;
    using Courier;
    using Courier.Pipeline;
    using GreenPipes;
    using GreenPipes.Contexts;
    using GreenPipes.Specifications;


    public static class ExecuteActivityPipeBuilder
    {
        public static IPipe<RequestContext> Build<TActivity, TArguments>(
            this IEnumerable<IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>> pipeSpecifications,
            IFilter<RequestContext<ExecuteActivityContext<TActivity, TArguments>>> consumeFilter)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var builder = new ExecuteActivityPipeBuilder<TActivity, TArguments>();
            foreach (IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>> specification in pipeSpecifications)
                specification.Apply(builder);

            ExecuteActivityPipeBuilder<TActivity, TArguments> builders = builder;
            if (builders == null)
                throw new InvalidOperationException("Should not be null, ever");

            return Pipe.New<RequestContext>(cfg =>
            {
                cfg.UseDispatch(new RequestConverterFactory(), d =>
                {
                    d.Handle<ExecuteActivityContext<TActivity, TArguments>>(h =>
                    {
                        AddFilters(builders, h);

                        h.UseFilter(consumeFilter);
                    });
                });
            });
        }

        static void AddFilters<TActivity, TArguments>(ExecuteActivityPipeBuilder<TActivity, TArguments> builders,
            IPipeConfigurator<RequestContext<ExecuteActivityContext<TActivity, TArguments>>> h)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            foreach (IFilter<ExecuteActivityContext<TActivity, TArguments>> filter in builders.Filters)
            {
                var filterSpecification = new FilterPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>(filter);

                var pipeSpecification = new SplitFilterPipeSpecification
                    <RequestContext<ExecuteActivityContext<TActivity, TArguments>>, ExecuteActivityContext<TActivity, TArguments>>(
                    filterSpecification, (input, context) => input, context => context.Request);

                h.AddPipeSpecification(pipeSpecification);
            }
        }
    }


    public class ExecuteActivityPipeBuilder<TActivity, TArguments> :
        IPipeBuilder<ExecuteActivityContext<TArguments>>,
        IPipeBuilder<ExecuteActivityContext<TActivity, TArguments>>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IList<IFilter<ExecuteActivityContext<TActivity, TArguments>>> _filters;

        public ExecuteActivityPipeBuilder()
        {
            _filters = new List<IFilter<ExecuteActivityContext<TActivity, TArguments>>>();
        }

        public IEnumerable<IFilter<ExecuteActivityContext<TActivity, TArguments>>> Filters => _filters;

        public void AddFilter(IFilter<ExecuteActivityContext<TActivity, TArguments>> filter)
        {
            _filters.Add(filter);
        }

        public void AddFilter(IFilter<ExecuteActivityContext<TArguments>> filter)
        {
            _filters.Add(new ExecuteActivitySplitFilter<TActivity, TArguments>(filter));
        }
    }
}
