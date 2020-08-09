namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using Autofac;


    class ActivatorUtils
    {
        readonly ConcurrentDictionary<Type, ParameterInfo[]> _cache = new ConcurrentDictionary<Type, ParameterInfo[]>();

        ActivatorUtils()
        {
        }

        static ActivatorUtils Instance { get; } = new ActivatorUtils();

        object[] GetParameters<T>(IComponentContext context)
            where T : class
        {
            ParameterInfo[] parameters = _cache.GetOrAdd(typeof(T), t => GetParameterInfos(t, context));
            var result = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                result[i] = parameter.IsOptional
                    ? context.ResolveOptional(parameter.ParameterType) ?? parameter.DefaultValue
                    : context.Resolve(parameter.ParameterType);
            }

            return result;
        }

        static ParameterInfo[] GetParameterInfos(Type instanceType, IComponentContext context)
        {
            var result = new ParameterInfo[0];

            var emptyConstructorIsExists = false;

            foreach (var constructor in instanceType
                .GetTypeInfo()
                .DeclaredConstructors
                .Where(c => !c.IsStatic && c.IsPublic))
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Length == 0)
                    emptyConstructorIsExists = true;

                if (!parameters.All(x => x.IsOptional || context.IsRegistered(x.ParameterType)))
                    continue;

                if (parameters.Length > result.Length)
                    result = parameters;
            }

            if (result.Length == 0 && !emptyConstructorIsExists)
                throw new ConfigurationException($"Can not resolve instance: {instanceType}");

            return result;
        }

        public static T GetOrCreateInstance<T>(IComponentContext context)
            where T : class
        {
            if (context.TryResolve<T>(out var instance))
                return instance;

            object[] parameters = Instance.GetParameters<T>(context);
            return (T)Activator.CreateInstance(typeof(T), parameters);
        }
    }
}
