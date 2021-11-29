namespace MassTransit
{
    using System;
    using Configuration;
    using Courier;
    using DependencyInjection.Registration;
    using Internals;


    public static class RegistrationConfiguratorExtensions
    {
        /// <summary>
        /// Adds the consumer, along with an optional consumer definition
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="consumerType">The consumer type</param>
        /// <param name="consumerDefinitionType">The consumer definition type</param>
        public static IConsumerRegistrationConfigurator AddConsumer(this IRegistrationConfigurator configurator, Type consumerType,
            Type consumerDefinitionType = null)
        {
            if (MessageTypeCache.HasSagaInterfaces(consumerType))
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


        interface IConfigureSagaRepository
        {
            void Configure(IRegistrationConfigurator configurator, ISagaRepositoryRegistrationProvider provider);
        }


        class ConfigureSagaRepository<TSaga> :
            IConfigureSagaRepository
            where TSaga : class, ISaga
        {
            public void Configure(IRegistrationConfigurator configurator, ISagaRepositoryRegistrationProvider provider)
            {
                var registrationConfigurator = new SagaRegistrationConfigurator<TSaga>(configurator);

                provider.Configure(registrationConfigurator);
            }
        }
    }
}
