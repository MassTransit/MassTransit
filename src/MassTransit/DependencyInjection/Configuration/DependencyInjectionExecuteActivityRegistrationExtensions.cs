namespace MassTransit.Configuration
{
    using System;
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
            return new ExecuteActivityRegistrar<TActivity, TArguments>().Register(collection, registrar);
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
            return new ExecuteActivityDefinitionRegistrar<TActivity, TArguments, TDefinition>().Register(collection, registrar);
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

            if (!activityDefinitionType.ClosesType(typeof(IExecuteActivityDefinition<,>), out Type[] types) || types[0] != typeof(TActivity))
            {
                throw new ArgumentException(
                    $"{TypeCache.GetShortName(activityDefinitionType)} is not an activity definition of {TypeCache<TActivity>.ShortName}",
                    nameof(activityDefinitionType));
            }

            var register = (IExecuteActivityRegistrar)Activator.CreateInstance(typeof(ExecuteActivityDefinitionRegistrar<,,>)
                .MakeGenericType(typeof(TActivity), typeof(TArguments), activityDefinitionType));

            return register.Register(collection, registrar);
        }

        public static IExecuteActivityRegistration RegisterExecuteActivity(this IServiceCollection collection, IContainerRegistrar registrar, Type activityType,
            Type activityDefinitionType = null)
        {
            if (activityType.ClosesType(typeof(IActivity<,>), out Type[] _))
            {
                throw new ArgumentException($"Activities must be registered using RegisterActivity: {TypeCache.GetShortName(activityType)}",
                    nameof(activityType));
            }

            if (!activityType.ClosesType(typeof(IExecuteActivity<>), out Type[] argumentTypes))
            {
                throw new ArgumentException($"Execute activities must implement IExecuteActivity<TArguments>: {TypeCache.GetShortName(activityType)}",
                    nameof(activityType));
            }

            if (activityDefinitionType != null)
            {
                if (!activityDefinitionType.ClosesType(typeof(IExecuteActivityDefinition<,>), out Type[] types) || types[0] != activityType)
                {
                    throw new ArgumentException(
                        $"{TypeCache.GetShortName(activityDefinitionType)} is not an activity definition of {TypeCache.GetShortName(activityType)}",
                        nameof(activityDefinitionType));
                }

                var activityRegistrar = (IExecuteActivityRegistrar)Activator.CreateInstance(typeof(ExecuteActivityDefinitionRegistrar<,,>)
                    .MakeGenericType(activityType, argumentTypes[0], activityDefinitionType));

                return activityRegistrar.Register(collection, registrar);
            }


            var register = (IExecuteActivityRegistrar)Activator.CreateInstance(typeof(ExecuteActivityRegistrar<,>)
                .MakeGenericType(activityType, argumentTypes[0]));

            return register.Register(collection, registrar);
        }


        interface IExecuteActivityRegistrar
        {
            IExecuteActivityRegistration Register(IServiceCollection collection, IContainerRegistrar registrar);
        }


        class ExecuteActivityRegistrar<TActivity, TArguments> :
            IExecuteActivityRegistrar
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            public virtual IExecuteActivityRegistration Register(IServiceCollection collection, IContainerRegistrar registrar)
            {
                collection.TryAddScoped<TActivity>();

                return registrar.GetOrAddRegistration<IExecuteActivityRegistration>(typeof(TActivity),
                    _ => new ExecuteActivityRegistration<TActivity, TArguments>(registrar));
            }
        }


        class ExecuteActivityDefinitionRegistrar<TActivity, TArguments, TDefinition> :
            ExecuteActivityRegistrar<TActivity, TArguments>
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            public override IExecuteActivityRegistration Register(IServiceCollection collection, IContainerRegistrar registrar)
            {
                var registration = base.Register(collection, registrar);

                registrar.AddDefinition<IExecuteActivityDefinition<TActivity, TArguments>, TDefinition>();

                return registration;
            }
        }
    }
}
