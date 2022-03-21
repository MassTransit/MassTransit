namespace MassTransit.Configuration
{
    using System;
    using Courier;
    using DependencyInjection;
    using DependencyInjection.Registration;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public static class DependencyInjectionActivityRegistrationExtensions
    {
        public static IActivityRegistration RegisterActivity<TActivity, TArguments, TLog>(this IServiceCollection collection)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            return RegisterActivity<TActivity, TArguments, TLog>(collection, new DependencyInjectionContainerRegistrar(collection));
        }

        public static IActivityRegistration RegisterActivity<TActivity, TArguments, TLog>(this IServiceCollection collection, IContainerRegistrar registrar)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            return new Activity<TActivity, TArguments, TLog>().Register(collection, registrar);
        }

        public static IActivityRegistration RegisterActivity<TActivity, TArguments, TLog, TDefinition>(this IServiceCollection collection)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
        {
            return RegisterActivity<TActivity, TArguments, TLog, TDefinition>(collection, new DependencyInjectionContainerRegistrar(collection));
        }

        public static IActivityRegistration RegisterActivity<TActivity, TArguments, TLog, TDefinition>(this IServiceCollection collection,
            IContainerRegistrar registrar)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
        {
            return new ActivityDefinition<TActivity, TArguments, TLog, TDefinition>().Register(collection, registrar);
        }

        public static IActivityRegistration RegisterActivity<TActivity, TArguments, TLog>(this IServiceCollection collection, Type activityDefinitionType)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            return RegisterActivity<TActivity, TArguments, TLog>(collection, new DependencyInjectionContainerRegistrar(collection), activityDefinitionType);
        }

        public static IActivityRegistration RegisterActivity<TActivity, TArguments, TLog>(this IServiceCollection collection, IContainerRegistrar registrar,
            Type activityDefinitionType)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            if (activityDefinitionType == null)
                return RegisterActivity<TActivity, TArguments, TLog>(collection, registrar);

            if (!activityDefinitionType.ClosesType(typeof(IActivityDefinition<,,>), out Type[] types) || types[0] != typeof(TActivity))
            {
                throw new ArgumentException(
                    $"{TypeCache.GetShortName(activityDefinitionType)} is not an activity definition of {TypeCache<TActivity>.ShortName}",
                    nameof(activityDefinitionType));
            }

            var register = (IRegister)Activator.CreateInstance(typeof(ActivityDefinition<,,,>)
                .MakeGenericType(typeof(TActivity), typeof(TArguments), typeof(TLog), activityDefinitionType));

            return register.Register(collection, registrar);
        }


        interface IRegister
        {
            IActivityRegistration Register(IServiceCollection collection, IContainerRegistrar registrar);
        }


        class Activity<TActivity, TArguments, TLog> :
            IRegister
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            public virtual IActivityRegistration Register(IServiceCollection collection, IContainerRegistrar registrar)
            {
                collection.TryAddScoped<TActivity>();

                collection.TryAddTransient<IExecuteActivityScopeProvider<TActivity, TArguments>,
                    ExecuteActivityScopeProvider<TActivity, TArguments>>();

                collection.TryAddTransient<ICompensateActivityScopeProvider<TActivity, TLog>,
                    CompensateActivityScopeProvider<TActivity, TLog>>();

                return registrar.GetOrAdd<IActivityRegistration>(typeof(TActivity), _ => new ActivityRegistration<TActivity, TArguments, TLog>());
            }
        }


        class ActivityDefinition<TActivity, TArguments, TLog, TDefinition> :
            Activity<TActivity, TArguments, TLog>
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            public override IActivityRegistration Register(IServiceCollection collection, IContainerRegistrar registrar)
            {
                var registration = base.Register(collection, registrar);

                collection.TryAddSingleton<TDefinition>();
                collection.TryAddSingleton<IActivityDefinition<TActivity, TArguments, TLog>>(provider => provider.GetRequiredService<TDefinition>());

                return registration;
            }
        }
    }
}
