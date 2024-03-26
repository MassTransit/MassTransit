namespace MassTransit
{
    using System;
    using Internals;
    using Metadata;


    public static class RegistrationConfiguratorExtensions
    {
        /// <summary>
        /// Adds the consumer, allowing configuration when it is configured on an endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The consumer type</typeparam>
        public static IConsumerRegistrationConfigurator<T> AddConsumer<T>(this IRegistrationConfigurator configurator,
            Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            return configure != null ? configurator.AddConsumer<T>((_, cfg) => configure.Invoke(cfg)) : configurator.AddConsumer<T>();
        }

        /// <summary>
        /// Adds the consumer, allowing configuration when it is configured on an endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="consumerDefinitionType">The consumer definition type</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The consumer type</typeparam>
        public static IConsumerRegistrationConfigurator<T> AddConsumer<T>(this IRegistrationConfigurator configurator,
            Type consumerDefinitionType, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            return configure != null
                ? configurator.AddConsumer<T>(consumerDefinitionType, (_, cfg) => configure.Invoke(cfg))
                : configurator.AddConsumer<T>(consumerDefinitionType);
        }

        /// <summary>
        /// Adds the saga, allowing configuration when it is configured on the endpoint. This should not
        /// be used for state machine (Automatonymous) sagas.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The saga type</typeparam>
        public static ISagaRegistrationConfigurator<T> AddSaga<T>(this IRegistrationConfigurator configurator,
            Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            return configure != null ? configurator.AddSaga<T>((_, cfg) => configure.Invoke(cfg)) : configurator.AddSaga<T>();
        }

        /// <summary>
        /// Adds the saga, allowing configuration when it is configured on the endpoint. This should not
        /// be used for state machine (Automatonymous) sagas.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="sagaDefinitionType">The saga definition type</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The saga type</typeparam>
        public static ISagaRegistrationConfigurator<T> AddSaga<T>(this IRegistrationConfigurator configurator, Type sagaDefinitionType,
            Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            return configure != null
                ? configurator.AddSaga<T>(sagaDefinitionType, (_, cfg) => configure.Invoke(cfg))
                : configurator.AddSaga<T>(sagaDefinitionType);
        }

        /// <summary>
        /// Adds a SagaStateMachine to the registry, using the factory method, and updates the registrar prior to registering so that the default
        /// saga registrar isn't notified.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TStateMachine"></typeparam>
        /// <typeparam name="T"></typeparam>
        public static ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(this IRegistrationConfigurator configurator,
            Action<ISagaConfigurator<T>> configure = null)
            where TStateMachine : class, SagaStateMachine<T>
            where T : class, SagaStateMachineInstance
        {
            return configure != null
                ? configurator.AddSagaStateMachine<TStateMachine, T>((_, cfg) => configure.Invoke(cfg))
                : configurator.AddSagaStateMachine<TStateMachine, T>();
        }

        /// <summary>
        /// Adds a SagaStateMachine to the registry, using the factory method, and updates the registrar prior to registering so that the default
        /// saga registrar isn't notified.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="sagaDefinitionType"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TStateMachine"></typeparam>
        /// <typeparam name="T"></typeparam>
        public static ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(this IRegistrationConfigurator configurator,
            Type sagaDefinitionType, Action<ISagaConfigurator<T>> configure = null)
            where TStateMachine : class, SagaStateMachine<T>
            where T : class, SagaStateMachineInstance
        {
            return configure != null
                ? configurator.AddSagaStateMachine<TStateMachine, T>(sagaDefinitionType, (_, cfg) => configure.Invoke(cfg))
                : configurator.AddSagaStateMachine<TStateMachine, T>(sagaDefinitionType);
        }

        /// <summary>
        /// Adds an execute activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        public static IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(
            this IRegistrationConfigurator configurator,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return configure != null
                ? configurator.AddExecuteActivity<TActivity, TArguments>((_, cfg) => configure.Invoke(cfg))
                : configurator.AddExecuteActivity<TActivity, TArguments>();
        }

        /// <summary>
        /// Adds an execute activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="executeActivityDefinitionType"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        public static IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(
            this IRegistrationConfigurator configurator, Type executeActivityDefinitionType,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return configure != null
                ? configurator.AddExecuteActivity<TActivity, TArguments>(executeActivityDefinitionType, (_, cfg) => configure.Invoke(cfg))
                : configurator.AddExecuteActivity<TActivity, TArguments>(executeActivityDefinitionType);
        }

        /// <summary>
        /// Adds an activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configureExecute">The execute configuration callback</param>
        /// <param name="configureCompensate">The compensate configuration callback</param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        /// <typeparam name="TLog">The log type</typeparam>
        public static IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(
            this IRegistrationConfigurator configurator,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute = null,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate = null)
            where TActivity : class, IActivity<TArguments, TLog>
            where TLog : class
            where TArguments : class
        {
            return configurator.AddActivity<TActivity, TArguments, TLog>((_, cfg) => configureExecute?.Invoke(cfg),
                (_, cfg) => configureCompensate?.Invoke(cfg));
        }

        /// <summary>
        /// Adds an activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="activityDefinitionType"></param>
        /// <param name="configureExecute">The execute configuration callback</param>
        /// <param name="configureCompensate">The compensate configuration callback</param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        /// <typeparam name="TLog">The log type</typeparam>
        public static IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(
            this IRegistrationConfigurator configurator, Type activityDefinitionType,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute = null,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate = null)
            where TActivity : class, IActivity<TArguments, TLog>
            where TLog : class
            where TArguments : class
        {
            return configurator.AddActivity<TActivity, TArguments, TLog>(activityDefinitionType, (_, cfg) => configureExecute?.Invoke(cfg),
                (_, cfg) => configureCompensate?.Invoke(cfg));
        }

        /// <summary>
        /// Adds the consumer, along with an optional consumer definition
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="consumerType">The consumer type</param>
        /// <param name="consumerDefinitionType">The consumer definition type</param>
        public static IConsumerRegistrationConfigurator AddConsumer(this IRegistrationConfigurator configurator, Type consumerType,
            Type consumerDefinitionType = null)
        {
            if (RegistrationMetadata.IsSaga(consumerType))
                throw new ArgumentException($"{TypeCache.GetShortName(consumerType)} is a saga, and cannot be registered as a consumer", nameof(consumerType));

            var register = (IRegisterConsumer)Activator.CreateInstance(typeof(RegisterConsumer<>).MakeGenericType(consumerType));

            return register.Register(configurator, consumerDefinitionType);
        }

        /// <summary>
        /// Adds the saga, along with an optional saga definition
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="sagaType">The saga type</param>
        /// <param name="sagaDefinitionType">The saga definition type</param>
        public static ISagaRegistrationConfigurator AddSaga(this IRegistrationConfigurator configurator, Type sagaType, Type sagaDefinitionType = null)
        {
            if (sagaType.HasInterface<SagaStateMachineInstance>())
                throw new ArgumentException($"State machine sagas must be registered using AddSagaStateMachine: {TypeCache.GetShortName(sagaType)}");

            var register = (IRegisterSaga)Activator.CreateInstance(typeof(RegisterSaga<>).MakeGenericType(sagaType));

            return register.Register(configurator, sagaDefinitionType);
        }

        /// <summary>
        /// Adds the state machine saga, along with an optional saga definition
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="sagaType">The saga type</param>
        /// <param name="sagaDefinitionType">The saga definition type</param>
        public static ISagaRegistrationConfigurator AddSagaStateMachine(this IRegistrationConfigurator configurator, Type sagaType,
            Type sagaDefinitionType = null)
        {
            if (!sagaType.ClosesType(typeof(SagaStateMachine<>), out Type[] types))
                throw new ArgumentException($"The type is not a saga state machine: {TypeCache.GetShortName(sagaType)}", nameof(sagaType));

            var register = (IRegisterSaga)Activator.CreateInstance(typeof(RegisterSagaStateMachine<,>).MakeGenericType(sagaType, types[0]));

            return register.Register(configurator, sagaDefinitionType);
        }

        /// <summary>
        /// Adds an activity (Courier), along with an optional activity definition
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="activityType"></param>
        /// <param name="activityDefinitionType"></param>
        public static IActivityRegistrationConfigurator AddActivity(this IRegistrationConfigurator configurator, Type activityType,
            Type activityDefinitionType = null)
        {
            if (!activityType.ClosesType(typeof(IActivity<,>), out Type[] types))
                throw new ArgumentException($"The type is not a Courier activity: {TypeCache.GetShortName(activityType)}", nameof(activityType));

            var register = (IRegisterActivity)Activator.CreateInstance(typeof(RegisterActivity<,,>).MakeGenericType(activityType, types[0], types[1]));

            return register.Register(configurator, activityDefinitionType);
        }

        /// <summary>
        /// Adds an execute activity (Courier), along with an optional activity definition
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="activityType"></param>
        /// <param name="activityDefinitionType"></param>
        public static IExecuteActivityRegistrationConfigurator AddExecuteActivity(this IRegistrationConfigurator configurator, Type activityType,
            Type activityDefinitionType = null)
        {
            if (!activityType.ClosesType(typeof(IExecuteActivity<>), out Type[] types))
                throw new ArgumentException($"The type is not a Courier execute activity: {TypeCache.GetShortName(activityType)}", nameof(activityType));

            var register = (IRegisterExecuteActivity)Activator.CreateInstance(typeof(RegisterExecuteActivity<,>).MakeGenericType(activityType, types[0]));

            return register.Register(configurator, activityDefinitionType);
        }

        /// <summary>
        /// Adds a future registration, along with an optional definition
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="futureType"></param>
        /// <param name="futureDefinitionType">The future definition type</param>
        public static IFutureRegistrationConfigurator AddFuture(this IRegistrationConfigurator configurator, Type futureType,
            Type futureDefinitionType = null)
        {
            if (!futureType.ClosesType(typeof(SagaStateMachine<>), out Type[] types) && types[0] == typeof(FutureState))
                throw new ArgumentException($"The type is not a future: {TypeCache.GetShortName(futureType)}", nameof(futureType));

            var register = (IRegisterFuture)Activator.CreateInstance(typeof(RegisterFuture<>).MakeGenericType(futureType));

            return register.Register(configurator, futureDefinitionType);
        }


        interface IRegisterConsumer
        {
            IConsumerRegistrationConfigurator Register(IRegistrationConfigurator configurator, Type consumerDefinitionType);
        }


        class RegisterConsumer<TConsumer> :
            IRegisterConsumer
            where TConsumer : class, IConsumer
        {
            public IConsumerRegistrationConfigurator Register(IRegistrationConfigurator configurator, Type consumerDefinitionType)
            {
                return configurator.AddConsumer<TConsumer>(consumerDefinitionType);
            }
        }


        interface IRegisterSaga
        {
            ISagaRegistrationConfigurator Register(IRegistrationConfigurator configurator, Type sagaDefinitionType);
        }


        class RegisterSaga<TSaga> :
            IRegisterSaga
            where TSaga : class, ISaga
        {
            public ISagaRegistrationConfigurator Register(IRegistrationConfigurator configurator, Type sagaDefinitionType)
            {
                return configurator.AddSaga<TSaga>(sagaDefinitionType);
            }
        }


        class RegisterSagaStateMachine<TStateMachine, TSaga> :
            IRegisterSaga
            where TStateMachine : class, SagaStateMachine<TSaga>
            where TSaga : class, SagaStateMachineInstance
        {
            public ISagaRegistrationConfigurator Register(IRegistrationConfigurator configurator, Type sagaDefinitionType)
            {
                return configurator.AddSagaStateMachine<TStateMachine, TSaga>(sagaDefinitionType);
            }
        }


        interface IRegisterActivity
        {
            IActivityRegistrationConfigurator Register(IRegistrationConfigurator configurator, Type activityDefinitionType);
        }


        class RegisterActivity<TActivity, TArguments, TLog> :
            IRegisterActivity
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            public IActivityRegistrationConfigurator Register(IRegistrationConfigurator configurator, Type activityDefinitionType)
            {
                return configurator.AddActivity<TActivity, TArguments, TLog>(activityDefinitionType);
            }
        }


        interface IRegisterExecuteActivity
        {
            IExecuteActivityRegistrationConfigurator Register(IRegistrationConfigurator configurator, Type activityDefinitionType);
        }


        class RegisterExecuteActivity<TActivity, TArguments> :
            IRegisterExecuteActivity
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            public IExecuteActivityRegistrationConfigurator Register(IRegistrationConfigurator configurator, Type activityDefinitionType)
            {
                return configurator.AddExecuteActivity<TActivity, TArguments>(activityDefinitionType);
            }
        }


        interface IRegisterFuture
        {
            IFutureRegistrationConfigurator Register(IRegistrationConfigurator configurator, Type futureDefinitionType);
        }


        class RegisterFuture<TFuture> :
            IRegisterFuture
            where TFuture : class, SagaStateMachine<FutureState>
        {
            public IFutureRegistrationConfigurator Register(IRegistrationConfigurator configurator, Type futureDefinitionType)
            {
                return configurator.AddFuture<TFuture>(futureDefinitionType);
            }
        }
    }
}
