namespace MassTransit.Configuration
{
    using System;
    using Courier;
    using DependencyInjection;
    using DependencyInjection.Registration;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public static class DependencyInjectionExecuteActivityRegistrationExtensions
    {
        public static IExecuteActivityRegistration RegisterExecuteActivity<TActivity, TArguments>(this IServiceCollection collection)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return RegisterExecuteActivity<TActivity, TArguments>(collection, new DependencyInjectionContainerRegistrar(collection));
        }

        public static IExecuteActivityRegistration RegisterExecuteActivity<TActivity, TArguments>(this IServiceCollection collection, IContainerRegistrar
            registrar)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return new Activity<TActivity, TArguments>().Register(collection, registrar);
        }

        public static IExecuteActivityRegistration RegisterExecuteActivity<TActivity, TArguments, TDefinition>(this IServiceCollection collection)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
        {
            return RegisterExecuteActivity<TActivity, TArguments, TDefinition>(collection, new DependencyInjectionContainerRegistrar(collection));
        }

        public static IExecuteActivityRegistration RegisterExecuteActivity<TActivity, TArguments, TDefinition>(this IServiceCollection collection,
            IContainerRegistrar registrar)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
        {
            return new ActivityDefinition<TActivity, TArguments, TDefinition>().Register(collection, registrar);
        }

        public static IExecuteActivityRegistration RegisterExecuteActivity<TActivity, TArguments>(this IServiceCollection collection, Type
            activityDefinitionType)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return RegisterExecuteActivity<TActivity, TArguments>(collection, new DependencyInjectionContainerRegistrar(collection), activityDefinitionType);
        }

        public static IExecuteActivityRegistration RegisterExecuteActivity<TActivity, TArguments>(this IServiceCollection collection,
            IContainerRegistrar registrar, Type activityDefinitionType)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            if (activityDefinitionType == null)
                return RegisterExecuteActivity<TActivity, TArguments>(collection, registrar);

            if (!activityDefinitionType.ClosesType(typeof(IActivityDefinition<,,>), out Type[] types) || types[0] != typeof(TActivity))
            {
                throw new ArgumentException(
                    $"{TypeCache.GetShortName(activityDefinitionType)} is not an activity definition of {TypeCache<TActivity>.ShortName}",
                    nameof(activityDefinitionType));
            }

            var register = (IRegister)Activator.CreateInstance(typeof(ActivityDefinition<,,>)
                .MakeGenericType(typeof(TActivity), typeof(TArguments), activityDefinitionType));

            return register.Register(collection, registrar);
        }


        interface IRegister
        {
            IExecuteActivityRegistration Register(IServiceCollection collection, IContainerRegistrar registrar);
        }


        class Activity<TActivity, TArguments> :
            IRegister
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            public virtual IExecuteActivityRegistration Register(IServiceCollection collection, IContainerRegistrar registrar)
            {
                collection.TryAddScoped<TActivity>();

                collection.TryAddTransient<IExecuteActivityScopeProvider<TActivity, TArguments>,
                    ExecuteActivityScopeProvider<TActivity, TArguments>>();

                return registrar.GetOrAdd<IExecuteActivityRegistration>(typeof(TActivity), _ => new ExecuteActivityRegistration<TActivity, TArguments>());
            }
        }


        class ActivityDefinition<TActivity, TArguments, TDefinition> :
            Activity<TActivity, TArguments>
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            public override IExecuteActivityRegistration Register(IServiceCollection collection, IContainerRegistrar registrar)
            {
                var registration = base.Register(collection, registrar);

                collection.TryAddSingleton<TDefinition>();
                collection.TryAddSingleton<IExecuteActivityDefinition<TActivity, TArguments>>(provider => provider.GetRequiredService<TDefinition>());

                return registration;
            }
        }
    }
}
