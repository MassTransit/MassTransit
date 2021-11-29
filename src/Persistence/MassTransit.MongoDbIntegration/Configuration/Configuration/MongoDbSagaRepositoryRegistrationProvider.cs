namespace MassTransit.Configuration
{
    using System;
    using Internals;


    public class MongoDbSagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        readonly Action<IMongoDbSagaRepositoryConfigurator> _configure;

        public MongoDbSagaRepositoryRegistrationProvider(Action<IMongoDbSagaRepositoryConfigurator> configure)
        {
            _configure = configure;
        }

        void ISagaRepositoryRegistrationProvider.Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class
        {
            if (typeof(TSaga).HasInterface<ISagaVersion>())
            {
                var proxy = (IProxy)Activator.CreateInstance(typeof(Proxy<>).MakeGenericType(typeof(TSaga)), configurator);

                proxy.Configure(this);
            }
        }

        protected virtual void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISagaVersion
        {
            configurator.MongoDbRepository(r => _configure?.Invoke(r));
        }


        interface IProxy
        {
            public void Configure<T>(T provider)
                where T : MongoDbSagaRepositoryRegistrationProvider;
        }


        class Proxy<TSaga> :
            IProxy
            where TSaga : class, ISagaVersion
        {
            readonly ISagaRegistrationConfigurator<TSaga> _configurator;

            public Proxy(ISagaRegistrationConfigurator<TSaga> configurator)
            {
                _configurator = configurator;
            }

            public void Configure<T>(T provider)
                where T : MongoDbSagaRepositoryRegistrationProvider
            {
                provider.Configure(_configurator);
            }
        }
    }
}
