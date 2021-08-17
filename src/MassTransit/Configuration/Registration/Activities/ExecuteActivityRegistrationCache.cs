namespace MassTransit.Registration
{
    using System;
    using System.Linq;
    using Courier;
    using Internals.Extensions;
    using Metadata;


    public static class ExecuteActivityRegistrationCache
    {
        public static void Register(Type activityType, IContainerRegistrar registrar)
        {
            Cached.Instance.GetOrAdd(activityType).Register(registrar);
        }

        public static IExecuteActivityRegistrationConfigurator AddExecuteActivity(IRegistrationConfigurator configurator, Type activityType,
            Type activityDefinitionType = null)
        {
            return Cached.Instance.GetOrAdd(activityType).AddExecuteActivity(configurator, activityDefinitionType);
        }

        static CachedRegistration Factory(Type activityType)
        {
            if (!activityType.HasInterface(typeof(IExecuteActivity<>)))
                throw new ArgumentException($"The type is not an execute activity: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));

            if (activityType.HasInterface(typeof(ICompensateActivity<>)))
            {
                throw new ArgumentException($"The type is an activity, which supports compensation: {TypeMetadataCache.GetShortName(activityType)}",
                    nameof(activityType));
            }

            var argumentType = activityType.GetClosingArguments(typeof(IExecuteActivity<>)).Single();

            return (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<,>).MakeGenericType(activityType, argumentType));
        }


        static class Cached
        {
            internal static readonly RegistrationCache<CachedRegistration> Instance = new RegistrationCache<CachedRegistration>(Factory);
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
            IExecuteActivityRegistrationConfigurator AddExecuteActivity(IRegistrationConfigurator configurator, Type activityDefinitionType);
        }


        class CachedRegistration<TActivity, TArguments> :
            CachedRegistration
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterExecuteActivity<TActivity, TArguments>();
            }

            public IExecuteActivityRegistrationConfigurator AddExecuteActivity(IRegistrationConfigurator configurator, Type activityDefinitionType)
            {
                IExecuteActivityRegistrationConfigurator<TActivity, TArguments> registrationConfigurator =
                    configurator.AddExecuteActivity<TActivity, TArguments>(activityDefinitionType);

                return registrationConfigurator;
            }
        }
    }
}
