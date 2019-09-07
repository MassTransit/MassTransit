namespace MassTransit.Builders
{
    using System;
    using System.Collections.Generic;
    using Courier;
    using Courier.Pipeline;
    using GreenPipes;
    using GreenPipes.Contexts;
    using GreenPipes.Specifications;


    public static class CompensateActivityPipeBuilder
    {
        public static IPipe<RequestContext> Build<TActivity, TLog>(
            this IEnumerable<IPipeSpecification<CompensateActivityContext<TActivity, TLog>>> pipeSpecifications,
            IFilter<RequestContext<CompensateActivityContext<TActivity, TLog>>> consumeFilter)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var builder = new CompensateActivityPipeBuilder<TActivity, TLog>();
            foreach (IPipeSpecification<CompensateActivityContext<TActivity, TLog>> specification in pipeSpecifications)
                specification.Apply(builder);

            CompensateActivityPipeBuilder<TActivity, TLog> builders = builder;
            if (builders == null)
                throw new InvalidOperationException("Should not be null, ever");

            return Pipe.New<RequestContext>(cfg =>
            {
                cfg.UseDispatch(new RequestConverterFactory(), d =>
                {
                    d.Handle<CompensateActivityContext<TActivity, TLog>>(h =>
                    {
                        AddFilters(builders, h);

                        h.UseFilter(consumeFilter);
                    });
                });
            });
        }

        static void AddFilters<TActivity, TLog>(CompensateActivityPipeBuilder<TActivity, TLog> builders,
            IPipeConfigurator<RequestContext<CompensateActivityContext<TActivity, TLog>>> h)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            foreach (IFilter<CompensateActivityContext<TActivity, TLog>> filter in builders.Filters)
            {
                var filterSpecification = new FilterPipeSpecification<CompensateActivityContext<TActivity, TLog>>(filter);

                var pipeSpecification = new SplitFilterPipeSpecification
                    <RequestContext<CompensateActivityContext<TActivity, TLog>>, CompensateActivityContext<TActivity, TLog>>(
                    filterSpecification, (input, context) => input, context => context.Request);

                h.AddPipeSpecification(pipeSpecification);
            }
        }
    }


    public class CompensateActivityPipeBuilder<TActivity, TLog> :
        IPipeBuilder<CompensateActivityContext<TLog>>,
        IPipeBuilder<CompensateActivityContext<TActivity, TLog>>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly IList<IFilter<CompensateActivityContext<TActivity, TLog>>> _filters;

        public CompensateActivityPipeBuilder()
        {
            _filters = new List<IFilter<CompensateActivityContext<TActivity, TLog>>>();
        }

        public IEnumerable<IFilter<CompensateActivityContext<TActivity, TLog>>> Filters => _filters;

        public void AddFilter(IFilter<CompensateActivityContext<TActivity, TLog>> filter)
        {
            _filters.Add(filter);
        }

        public void AddFilter(IFilter<CompensateActivityContext<TLog>> filter)
        {
            _filters.Add(new CompensateActivitySplitFilter<TActivity, TLog>(filter));
        }
    }
}
