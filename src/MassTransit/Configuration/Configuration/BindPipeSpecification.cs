namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Context;
    using Middleware;


    public class BindPipeSpecification<TLeft, TRight> :
        IPipeSpecification<TLeft>,
        IBindConfigurator<TLeft, TRight>
        where TLeft : class, PipeContext
        where TRight : class, PipeContext
    {
        readonly IPipeConfigurator<TLeft> _contextPipeConfigurator;
        readonly IBuildPipeConfigurator<BindContext<TLeft, TRight>> _pipeConfigurator;
        readonly IPipeContextSource<TRight, TLeft> _source;

        public BindPipeSpecification(IPipeContextSource<TRight, TLeft> source)
        {
            _source = source;
            _pipeConfigurator = new PipeConfigurator<BindContext<TLeft, TRight>>();
            _contextPipeConfigurator = new ContextPipeConfigurator(_pipeConfigurator);
        }

        IPipeConfigurator<TLeft> IBindConfigurator<TLeft, TRight>.ContextPipe => _contextPipeConfigurator;

        void IPipeConfigurator<BindContext<TLeft, TRight>>.AddPipeSpecification(IPipeSpecification<BindContext<TLeft, TRight>> specification)
        {
            _pipeConfigurator.AddPipeSpecification(specification);
        }

        void IPipeSpecification<TLeft>.Apply(IPipeBuilder<TLeft> builder)
        {
            IPipe<BindContext<TLeft, TRight>> pipe = _pipeConfigurator.Build();

            var bindFilter = new PipeContextSourceBindFilter<TLeft, TRight>(pipe, _source);

            builder.AddFilter(bindFilter);
        }

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            if (_source == null)
                yield return this.Failure("PipeContextSource", "must not be null");

            foreach (var result in _pipeConfigurator.Validate())
                yield return result;
        }


        class ContextPipeConfigurator :
            IPipeConfigurator<TLeft>
        {
            readonly IPipeConfigurator<BindContext<TLeft, TRight>> _configurator;

            public ContextPipeConfigurator(IPipeConfigurator<BindContext<TLeft, TRight>> configurator)
            {
                _configurator = configurator;
            }

            public void AddPipeSpecification(IPipeSpecification<TLeft> specification)
            {
                BindContext<TLeft, TRight> ContextProvider(BindContext<TLeft, TRight> input, TLeft context)
                {
                    return context as BindContext<TLeft, TRight> ?? new BindContextProxy<TLeft, TRight>(context, input.Right);
                }

                _configurator.AddPipeSpecification(new SplitFilterPipeSpecification<BindContext<TLeft, TRight>, TLeft>(specification,
                    ContextProvider,
                    context => context.Left));
            }
        }
    }
}
