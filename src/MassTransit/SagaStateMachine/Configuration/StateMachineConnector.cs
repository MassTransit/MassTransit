namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Util;


    public class StateMachineConnector<TInstance> :
        ISagaConnector
        where TInstance : class, ISaga, SagaStateMachineInstance
    {
        readonly List<ISagaMessageConnector<TInstance>> _connectors;
        readonly SagaStateMachine<TInstance> _stateMachine;

        public StateMachineConnector(SagaStateMachine<TInstance> stateMachine)
        {
            _stateMachine = stateMachine;

            try
            {
                _connectors = StateMachineEvents().ToList();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException($"Failed to create the state machine connector for {TypeCache<TInstance>.ShortName}", ex);
            }
        }

        public ISagaSpecification<T> CreateSagaSpecification<T>()
            where T : class, ISaga
        {
            List<ISagaMessageSpecification<TInstance>> messageSpecifications =
                _connectors.Select(x => x.CreateSagaMessageSpecification())
                    .ToList();

            var specification = new StateMachineSagaSpecification<TInstance>(_stateMachine, messageSpecifications);

            return specification as ISagaSpecification<T> ?? throw new ArgumentException("The generic argument did not match the connector type", nameof(T));
        }

        public ConnectHandle ConnectSaga<T>(IConsumePipeConnector consumePipe, ISagaRepository<T> sagaRepository, ISagaSpecification<T> specification)
            where T : class, ISaga
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (ISagaMessageConnector<T> connector in _connectors.Cast<ISagaMessageConnector<T>>())
                {
                    var handle = connector.ConnectSaga(consumePipe, sagaRepository, specification);

                    handles.Add(handle);
                }

                return new MultipleConnectHandle(handles);
            }
            catch (Exception)
            {
                foreach (var handle in handles)
                    handle.Dispose();

                throw;
            }
        }

        IEnumerable<ISagaMessageConnector<TInstance>> StateMachineEvents()
        {
            EventCorrelation[] correlations = _stateMachine.Correlations.ToArray();

            correlations.SelectMany(x => x.Validate()).ThrowIfContainsFailure("The state machine was not properly configured:");

            foreach (var correlation in correlations)
            {
                if (correlation.DataType.GetTypeInfo().IsValueType)
                    continue;

                var genericType = typeof(StateMachineInterfaceType<,>).MakeGenericType(typeof(TInstance), correlation.DataType);

                var interfaceType = (IStateMachineInterfaceType)Activator.CreateInstance(genericType, _stateMachine, correlation);

                yield return interfaceType.GetConnector<TInstance>();
            }
        }
    }
}
