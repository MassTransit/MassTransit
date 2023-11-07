namespace MassTransit.Configuration.Configuration
{
    using System;
    using Internals;


    public class CassandraDbSagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        readonly Action<ICassandraDbSagaRepositoryConfigurator> _configure;

        public CassandraDbSagaRepositoryRegistrationProvider(Action<ICassandraDbSagaRepositoryConfigurator> configure)
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
            configurator.CassandraDbRepository(r => _configure?.Invoke(r));
        }


        interface IProxy
        {
            public void Configure<T>(T provider)
                where T : CassandraDbSagaRepositoryRegistrationProvider;
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
                where T : CassandraDbSagaRepositoryRegistrationProvider
            {
                provider.Configure(_configurator);
            }
        }
    }
}
