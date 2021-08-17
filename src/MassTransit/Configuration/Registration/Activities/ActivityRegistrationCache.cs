namespace MassTransit.Registration
{
    using System;
    using System.Linq;
    using Courier;
    using Internals.Extensions;
    using Metadata;


    public static class ActivityRegistrationCache
    {
        public static void Register(Type activityType, IContainerRegistrar registrar)
        {
            Cached.Instance.GetOrAdd(activityType).Register(registrar);
        }

        public static IActivityRegistrationConfigurator AddActivity(IRegistrationConfigurator configurator, Type activityType,
            Type activityDefinitionType = null)
        {
            return Cached.Instance.GetOrAdd(activityType).AddActivity(configurator, activityDefinitionType);
        }

        static CachedRegistration Factory(Type activityType)
        {
            if (!activityType.HasInterface(typeof(IActivity<,>)))
                throw new ArgumentException($"The type is not an activity: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));

            Type[] argumentLogTypes = activityType.GetClosingArguments(typeof(IActivity<,>)).ToArray();
            var genericType = typeof(CachedRegistration<,,>).MakeGenericType(activityType, argumentLogTypes[0], argumentLogTypes[1]);

            return (CachedRegistration)Activator.CreateInstance(genericType);
        }


        static class Cached
        {
            internal static readonly RegistrationCache<CachedRegistration> Instance = new RegistrationCache<CachedRegistration>(Factory);
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
            IActivityRegistrationConfigurator AddActivity(IRegistrationConfigurator configurator, Type activityDefinitionType);
        }


        class CachedRegistration<TActivity, TArguments, TLog> :
            CachedRegistration
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterExecuteActivity<TActivity, TArguments>();
                registrar.RegisterCompensateActivity<TActivity, TLog>();
            }

            public IActivityRegistrationConfigurator AddActivity(IRegistrationConfigurator configurator, Type activityDefinitionType)
            {
                IActivityRegistrationConfigurator<TActivity, TArguments, TLog> registrationConfigurator =
                    configurator.AddActivity<TActivity, TArguments, TLog>(activityDefinitionType);

                return registrationConfigurator;
            }
        }
    }
}
