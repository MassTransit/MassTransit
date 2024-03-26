namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;


    public partial class PipeConfigurator<TContext> :
        IBuildPipeConfigurator<TContext>
        where TContext : class, PipeContext
    {
        readonly List<IPipeSpecification<TContext>> _specifications;

        public PipeConfigurator()
        {
            _specifications = new List<IPipeSpecification<TContext>>(16);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_specifications.Count == 0)
                yield break;

            for (var i = 0; i < _specifications.Count; i++)
            {
                foreach (var result in _specifications[i].Validate())
                    yield return result;
            }
        }

        public void AddPipeSpecification(IPipeSpecification<TContext> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        public IPipe<TContext> Build()
        {
            if (_specifications.Count == 0)
                return Cache.EmptyPipe;

            var builder = new PipeBuilder(_specifications.Count);

            var count = _specifications.Count;
            for (var index = 0; index < count; index++)
                _specifications[index].Apply(builder);

            return builder.Build();
        }

        public void Method1()
        {
        }

        public void Method2()
        {
        }
    }
}
