namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Saga;


    public class SagaMetadataCache<TSaga> :
        ISagaMetadataCache<TSaga>
        where TSaga : class, ISaga
    {
        readonly SagaInterfaceType[] _initiatedByOrOrchestratesTypes;
        readonly SagaInterfaceType[] _initiatedByTypes;
        readonly SagaInterfaceType[] _observesTypes;
        readonly SagaInterfaceType[] _orchestratesTypes;
        SagaInstanceFactoryMethod<TSaga> _factoryMethod;

        SagaMetadataCache()
        {
            _initiatedByTypes = GetInitiatingTypes().ToArray();
            _orchestratesTypes = GetOrchestratingTypes().ToArray();
            _observesTypes = GetObservingTypes().ToArray();
            _initiatedByOrOrchestratesTypes = GetInitiatingOrOrchestratingTypes().ToArray();

            GetActivatorSagaInstanceFactoryMethod();
        }

        public static SagaInterfaceType[] InitiatedByTypes => Cached.Instance.Value.InitiatedByTypes;
        public static SagaInterfaceType[] OrchestratesTypes => Cached.Instance.Value.OrchestratesTypes;
        public static SagaInterfaceType[] ObservesTypes => Cached.Instance.Value.ObservesTypes;
        public static SagaInterfaceType[] InitiatedByOrOrchestratesTypes => Cached.Instance.Value.InitiatedByOrOrchestratesTypes;
        public static SagaInstanceFactoryMethod<TSaga> FactoryMethod => Cached.Instance.Value.FactoryMethod;
        SagaInstanceFactoryMethod<TSaga> ISagaMetadataCache<TSaga>.FactoryMethod => _factoryMethod;
        SagaInterfaceType[] ISagaMetadataCache<TSaga>.InitiatedByTypes => _initiatedByTypes;
        SagaInterfaceType[] ISagaMetadataCache<TSaga>.OrchestratesTypes => _orchestratesTypes;
        SagaInterfaceType[] ISagaMetadataCache<TSaga>.ObservesTypes => _observesTypes;
        SagaInterfaceType[] ISagaMetadataCache<TSaga>.InitiatedByOrOrchestratesTypes => _initiatedByOrOrchestratesTypes;

        void GetActivatorSagaInstanceFactoryMethod()
        {
            var constructorInfo = typeof(TSaga).GetConstructor(new[] { typeof(Guid) });
            if (constructorInfo != null)
            {
                // this takes zero compilation time and speeds up application startup time
                // while the optimized method is generated asynchronously
                _factoryMethod = correlationId => (TSaga)Activator.CreateInstance(typeof(TSaga), correlationId);

                Task.Run(GenerateFactoryMethodAsynchronously);
            }
            else
            {
                constructorInfo = typeof(TSaga).GetConstructor(Type.EmptyTypes);
                var propertyInfo = typeof(TSaga).GetProperty("CorrelationId", typeof(Guid));
                if (constructorInfo != null && propertyInfo != null)
                {
                    // this takes zero compilation time and speeds up application startup time
                    // while the optimized method is generated asynchronously
                    _factoryMethod = correlationId =>
                    {
                        var saga = (TSaga)Activator.CreateInstance(typeof(TSaga));

                        propertyInfo.SetValue(saga, correlationId);

                        return saga;
                    };

                    Task.Run(GeneratePropertyFactoryMethodAsynchronously);
                }
                else
                {
                    throw new ConfigurationException(
                        $"The saga {TypeCache<TSaga>.ShortName} must have either a default constructor and a writable CorrelationId property or a constructor with a single Guid argument to assign the CorrelationId");
                }
            }
        }

        /// <summary>
        /// Creates a task to generate a compiled saga factory method that is faster than the
        /// regular Activator, but doing this asynchronously ensures we don't slow down startup
        /// </summary>
        /// <returns></returns>
        async Task GenerateFactoryMethodAsynchronously()
        {
            try
            {
                var factory = new ConstructorSagaInstanceFactory<TSaga>();

                Interlocked.Exchange(ref _factoryMethod, factory.FactoryMethod);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Generate constructor instance factory faulted: {SagaType}", TypeCache<TSaga>.ShortName);
            }
        }

        /// <summary>
        /// Creates a task to generate a compiled saga factory method that is faster than the
        /// regular Activator, but doing this asynchronously ensures we don't slow down startup
        /// </summary>
        /// <returns></returns>
        async Task GeneratePropertyFactoryMethodAsynchronously()
        {
            try
            {
                var factory = new PropertySagaInstanceFactory<TSaga>();

                Interlocked.Exchange(ref _factoryMethod, factory.FactoryMethod);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Generate property instance factory faulted: {SagaType}", TypeCache<TSaga>.ShortName);
            }
        }

        static IEnumerable<SagaInterfaceType> GetInitiatingTypes()
        {
            return typeof(TSaga).GetInterfaces()
                .Where(x => x.GetTypeInfo().IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(InitiatedBy<>))
                .Select(x => new SagaInterfaceType(x, x.GetGenericArguments()[0], typeof(TSaga)))
                .Where(x => MessageTypeCache.IsValidMessageType(x.MessageType));
        }

        static IEnumerable<SagaInterfaceType> GetOrchestratingTypes()
        {
            return typeof(TSaga).GetInterfaces()
                .Where(x => x.GetTypeInfo().IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Orchestrates<>))
                .Select(x => new SagaInterfaceType(x, x.GetGenericArguments()[0], typeof(TSaga)))
                .Where(x => MessageTypeCache.IsValidMessageType(x.MessageType));
        }

        static IEnumerable<SagaInterfaceType> GetObservingTypes()
        {
            return typeof(TSaga).GetInterfaces()
                .Where(x => x.GetTypeInfo().IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Observes<,>))
                .Select(x => new SagaInterfaceType(x, x.GetGenericArguments()[0], typeof(TSaga)))
                .Where(x => MessageTypeCache.IsValidMessageType(x.MessageType));
        }

        static IEnumerable<SagaInterfaceType> GetInitiatingOrOrchestratingTypes()
        {
            return typeof(TSaga).GetInterfaces()
                .Where(x => x.GetTypeInfo().IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(InitiatedByOrOrchestrates<>))
                .Select(x => new SagaInterfaceType(x, x.GetGenericArguments()[0], typeof(TSaga)))
                .Where(x => MessageTypeCache.IsValidMessageType(x.MessageType));
        }


        static class Cached
        {
            internal static readonly Lazy<ISagaMetadataCache<TSaga>> Instance = new Lazy<ISagaMetadataCache<TSaga>>(
                () => new SagaMetadataCache<TSaga>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
