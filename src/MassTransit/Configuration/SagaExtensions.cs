namespace MassTransit
{
    using System;
    using Configuration;


    public static class SagaExtensions
    {
        /// <summary>
        /// Configure a saga subscription
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="sagaRepository"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, ISagaRepository<T> sagaRepository,
            Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (sagaRepository == null)
                throw new ArgumentNullException(nameof(sagaRepository));

            LogContext.Debug?.Log("Subscribing Saga: {SagaType}", TypeCache<T>.ShortName);

            var sagaConfigurator = new SagaConfigurator<T>(sagaRepository, configurator);

            configure?.Invoke(sagaConfigurator);

            configurator.AddEndpointSpecification(sagaConfigurator);
        }

        /// <summary>
        /// Connects the saga to the bus
        /// </summary>
        /// <typeparam name="T">The saga type</typeparam>
        /// <param name="connector">The bus to which the saga is to be connected</param>
        /// <param name="sagaRepository">The saga repository</param>
        /// <param name="pipeSpecifications"></param>
        public static ConnectHandle ConnectSaga<T>(this IConsumePipeConnector connector, ISagaRepository<T> sagaRepository,
            params IPipeSpecification<SagaConsumeContext<T>>[] pipeSpecifications)
            where T : class, ISaga
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            if (sagaRepository == null)
                throw new ArgumentNullException(nameof(sagaRepository));

            LogContext.Debug?.Log("Connecting Saga: {SagaType}", TypeCache<T>.ShortName);

            ISagaSpecification<T> specification = SagaConnectorCache<T>.Connector.CreateSagaSpecification<T>();
            foreach (IPipeSpecification<SagaConsumeContext<T>> pipeSpecification in pipeSpecifications)
                specification.AddPipeSpecification(pipeSpecification);

            return SagaConnectorCache<T>.Connector.ConnectSaga(connector, sagaRepository, specification);
        }
    }
}
