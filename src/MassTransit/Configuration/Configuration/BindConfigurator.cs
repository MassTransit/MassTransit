namespace MassTransit.Configuration
{
    using System;


    public class BindConfigurator<TLeft> :
        IBindConfigurator<TLeft>
        where TLeft : class, PipeContext
    {
        readonly IPipeConfigurator<TLeft> _configurator;

        public BindConfigurator(IPipeConfigurator<TLeft> configurator)
        {
            _configurator = configurator;
        }

        void IBindConfigurator<TLeft>.Source<T>(IPipeContextSource<T, TLeft> source, Action<IBindConfigurator<TLeft, T>> configureTarget)
        {
            var specification = new BindPipeSpecification<TLeft, T>(source);

            configureTarget?.Invoke(specification);

            _configurator.AddPipeSpecification(specification);
        }
    }
}
