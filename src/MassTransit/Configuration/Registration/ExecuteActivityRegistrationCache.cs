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

        public static IExecuteActivityRegistration CreateRegistration(Type activityType, Type activityDefinitionType, IContainerRegistrar registrar)
        {
            return Cached.Instance.GetOrAdd(activityType).CreateRegistration(activityDefinitionType, registrar);
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
            IExecuteActivityRegistration CreateRegistration(Type activityDefinitionType, IContainerRegistrar registrar);
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

            public IExecuteActivityRegistration CreateRegistration(Type activityDefinitionType, IContainerRegistrar registrar)
            {
                Register(registrar);

                if (activityDefinitionType != null)
                    ExecuteActivityDefinitionRegistrationCache.Register(activityDefinitionType, registrar);

                return new ExecuteActivityRegistration<TActivity, TArguments>();
            }
        }
    }
}
