namespace MassTransit.Metadata
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Courier;
    using Futures;
    using Internals;


    public static class RegistrationMetadata
    {
        /// <summary>
        /// Returns true if the type is a consumer, or a consumer definition
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsConsumerOrDefinition(Type type)
        {
            Type[] interfaces = type.GetTypeInfo().GetInterfaces();

            return interfaces.Any(t => InterfaceExtensions.HasInterface(t, typeof(IConsumer<>))
                || InterfaceExtensions.HasInterface(t, typeof(IJobConsumer<>))
                || InterfaceExtensions.HasInterface(t, typeof(IConsumerDefinition<>)));
        }

        /// <summary>
        /// Returns true if the type is a saga, or a saga definition
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSagaOrDefinition(Type type)
        {
            Type[] interfaces = type.GetTypeInfo().GetInterfaces();

            if (interfaces.Contains(typeof(ISaga)))
                return true;

            return interfaces.Any(t => t.HasInterface(typeof(InitiatedBy<>))
                || t.HasInterface(typeof(Orchestrates<>))
                || t.HasInterface(typeof(InitiatedByOrOrchestrates<>))
                || t.HasInterface(typeof(Observes<,>))
                || t.HasInterface(typeof(ISagaDefinition<>)));
        }

        /// <summary>
        /// Returns true if the type is a state machine or saga definition
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSagaStateMachineOrDefinition(Type type)
        {
            Type[] interfaces = type.GetTypeInfo().GetInterfaces();

            return interfaces.Any(t => t.HasInterface(typeof(SagaStateMachine<>))
                || t.HasInterface(typeof(ISagaDefinition<>)));
        }

        /// <summary>
        /// Returns true if the type is an activity
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsActivityOrDefinition(Type type)
        {
            Type[] interfaces = type.GetTypeInfo().GetInterfaces();

            return interfaces.Any(t => t.HasInterface(typeof(IExecuteActivity<>))
                || t.HasInterface(typeof(ICompensateActivity<>))
                || t.HasInterface(typeof(IActivityDefinition<,,>))
                || t.HasInterface(typeof(IExecuteActivityDefinition<,>)));
        }

        /// <summary>
        /// Returns true if the type is a future or future definition
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsFutureOrDefinition(Type type)
        {
            Type[] interfaces = type.GetTypeInfo().GetInterfaces();

            return interfaces.Any(t => t.HasInterface(typeof(SagaStateMachine<FutureState>))
                || t.HasInterface(typeof(IFutureDefinition<>)));
        }
    }
}
