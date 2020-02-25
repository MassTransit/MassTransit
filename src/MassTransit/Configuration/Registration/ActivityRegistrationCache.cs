namespace MassTransit.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Courier;
    using Internals.Extensions;
    using Metadata;


    public static class ActivityRegistrationCache
    {
        static CachedRegistration GetOrAdd(Type activityType)
        {
            if (!activityType.HasInterface(typeof(IActivity<,>)))
                throw new ArgumentException($"The type is not an activity: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));

            var argumentLogTypes = activityType.GetClosingArguments(typeof(IActivity<,>)).ToArray();
            var genericType = typeof(CachedRegistration<,,>).MakeGenericType(activityType, argumentLogTypes[0], argumentLogTypes[1]);

            return Cached.Instance.GetOrAdd(activityType, _ => (CachedRegistration)Activator.CreateInstance(genericType));
        }

        public static void Register(Type activityType, IContainerRegistrar registrar)
        {
            GetOrAdd(activityType).Register(registrar);
        }

        public static IActivityRegistration CreateRegistration(Type activityType, Type activityDefinitionType, IContainerRegistrar registrar)
        {
            return GetOrAdd(activityType).CreateRegistration(activityDefinitionType, registrar);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedRegistration> Instance = new ConcurrentDictionary<Type, CachedRegistration>();
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
            IActivityRegistration CreateRegistration(Type activityDefinitionType, IContainerRegistrar registrar);
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

            public IActivityRegistration CreateRegistration(Type activityDefinitionType, IContainerRegistrar registrar)
            {
                Register(registrar);

                if (activityDefinitionType != null)
                    ActivityDefinitionRegistrationCache.Register(activityDefinitionType, registrar);

                return new ActivityRegistration<TActivity, TArguments, TLog>();
            }
        }
    }
}
