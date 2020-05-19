namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;
    using Hangfire;
    using Internals.Reflection;
    using Metadata;


    public class MassTransitJobActivator :
        JobActivator
    {
        readonly IBus _bus;
        readonly ConcurrentDictionary<Type, IMassTransitJobActivatorFactory> _typeFactories;

        public MassTransitJobActivator(IBus bus)
        {
            _bus = bus;
            _typeFactories = new ConcurrentDictionary<Type, IMassTransitJobActivatorFactory>();
        }

        public override object ActivateJob(Type jobType) =>
            _typeFactories.GetOrAdd(jobType, CreateJobFactory)
                .ActivateJob();

        IMassTransitJobActivatorFactory CreateJobFactory(Type type)
        {
            var genericType = typeof(MassTransitJobActivatorFactory<>).MakeGenericType(type);

            return (IMassTransitJobActivatorFactory)Activator.CreateInstance(genericType, _bus);
        }


        interface IMassTransitJobActivatorFactory
        {
            object ActivateJob();
        }


        class MassTransitJobActivatorFactory<T> :
            IMassTransitJobActivatorFactory
        {
            readonly IBus _bus;
            readonly Func<IBus, T> _factory;

            public MassTransitJobActivatorFactory(IBus bus)
            {
                _bus = bus;
                _factory = CreateConstructor();
            }

            public object ActivateJob() => NewJob();

            T NewJob()
            {
                try
                {
                    return _factory(_bus);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Problem instantiating class '{TypeMetadataCache<T>.ShortName}'", ex);
                }
            }

            static Func<IBus, T> CreateConstructor()
            {
                var ctor = typeof(T).GetConstructor(new[] {typeof(IBus)});
                if (ctor != null)
                    return CreateServiceBusConstructor(ctor);

                ctor = typeof(T).GetConstructor(Type.EmptyTypes);
                if (ctor != null)
                    return CreateDefaultConstructor(ctor);

                throw new Exception($"The job class does not have a supported constructor: {TypeMetadataCache<T>.ShortName}");
            }

            static Func<IBus, T> CreateDefaultConstructor(ConstructorInfo constructorInfo)
            {
                var bus = Expression.Parameter(typeof(IBus), "bus");
                var @new = Expression.New(constructorInfo);

                return Expression.Lambda<Func<IBus, T>>(@new, bus).CompileFast();
            }

            static Func<IBus, T> CreateServiceBusConstructor(ConstructorInfo constructorInfo)
            {
                var bus = Expression.Parameter(typeof(IBus), "bus");
                var @new = Expression.New(constructorInfo, bus);

                return Expression.Lambda<Func<IBus, T>>(@new, bus).CompileFast();
            }
        }
    }
}
