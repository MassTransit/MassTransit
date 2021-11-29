namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Middleware;


    public class DispatchPipeSpecification<TInput> :
        IPipeSpecification<TInput>,
        IDispatchConfigurator<TInput>
        where TInput : class, PipeContext
    {
        readonly IPipeContextConverterFactory<TInput> _pipeContextConverterFactory;
        readonly IList<IPipeConnectorSpecification> _specifications;

        public DispatchPipeSpecification(IPipeContextConverterFactory<TInput> pipeContextConverterFactory)
        {
            _pipeContextConverterFactory = pipeContextConverterFactory;

            _specifications = new List<IPipeConnectorSpecification>();
        }

        public void Pipe<T>(Action<IPipeConfigurator<T>> configurePipe)
            where T : class, PipeContext
        {
            var specification = new ConfiguratorPipeConnectorSpecification<T>();

            configurePipe?.Invoke(specification);

            _specifications.Add(specification);
        }

        public void Apply(IPipeBuilder<TInput> builder)
        {
            var dynamicFilter = new DynamicFilter<TInput>(_pipeContextConverterFactory);

            var count = _specifications.Count;
            for (var index = 0; index < count; index++)
                _specifications[index].Connect(dynamicFilter);

            builder.AddFilter(dynamicFilter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_pipeContextConverterFactory == null)
                yield return this.Failure("PipeContextProviderFactory", "must not be null");

            foreach (var result in _specifications.SelectMany(x => x.Validate()))
                yield return result.WithParentKey("Dispatch");
        }
    }
}
