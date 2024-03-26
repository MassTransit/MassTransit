namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Metadata;
    using Util;


    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        internal class StateMachineConnector :
            ISagaConnector
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

                var specification = new StateMachineSagaSpecification(_stateMachine, messageSpecifications);

                return specification as ISagaSpecification<T> ??
                    throw new ArgumentException("The generic argument did not match the connector type", nameof(T));
            }

            public ConnectHandle ConnectSaga<T>(IConsumePipeConnector consumePipe, ISagaRepository<T> sagaRepository, ISagaSpecification<T> specification)
                where T : class, ISaga
            {
                var handles = new List<ConnectHandle>(_connectors.Count);
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

                var factory = new Factory();

                foreach (var correlation in correlations)
                {
                    if (correlation.DataType.IsValueType)
                        continue;

                    var interfaceType = Activation.Activate(correlation.DataType, factory, _stateMachine, correlation);

                    yield return interfaceType.GetConnector<TInstance>();
                }
            }
        }


        readonly struct Factory :
            IActivationType<IStateMachineInterfaceType, SagaStateMachine<TInstance>, EventCorrelation>
        {
            public IStateMachineInterfaceType ActivateType<T>(SagaStateMachine<TInstance> machine, EventCorrelation correlation)
                where T : class
            {
                return new StateMachineInterfaceType<TInstance, T>(machine, (EventCorrelation<TInstance, T>)correlation);
            }
        }
    }
}
