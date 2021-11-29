namespace MassTransit
{
    using System;
    using Configuration;


    public static class SagaPipeConfiguratorExtensions
    {
        /// <summary>
        /// Adds a filter to the pipe
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        /// <typeparam name="TSaga"></typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="filter">The already built pipe</param>
        public static void UseFilter<TSaga, T>(this IPipeConfigurator<SagaConsumeContext<TSaga, T>> configurator,
            IFilter<SagaConsumeContext<TSaga>> filter)
            where T : class
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new SagaFilterSpecification<TSaga, T>(filter);

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }
    }
}
