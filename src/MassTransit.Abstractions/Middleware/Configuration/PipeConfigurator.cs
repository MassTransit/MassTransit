namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class PipeConfigurator<TContext> :
        IBuildPipeConfigurator<TContext>
        where TContext : class, PipeContext
    {
        readonly List<IPipeSpecification<TContext>> _specifications;

        public PipeConfigurator()
        {
            _specifications = new List<IPipeSpecification<TContext>>(4);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.Count == 0
                ? Array.Empty<ValidationResult>()
                : _specifications.SelectMany(x => x.Validate());
        }

        void IPipeConfigurator<TContext>.AddPipeSpecification(IPipeSpecification<TContext> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        public IPipe<TContext> Build()
        {
            if (_specifications.Count == 0)
                return Pipe.Empty<TContext>();

            var builder = new PipeBuilder<TContext>(_specifications.Count);

            var count = _specifications.Count;
            for (var index = 0; index < count; index++)
                _specifications[index].Apply(builder);

            return builder.Build();
        }
    }
}
