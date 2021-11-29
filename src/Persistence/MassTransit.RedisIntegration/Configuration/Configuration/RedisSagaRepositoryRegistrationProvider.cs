namespace MassTransit.Configuration
{
    using System;
    using Internals;


    public class RedisSagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        readonly Action<IRedisSagaRepositoryConfigurator> _configure;

        public RedisSagaRepositoryRegistrationProvider(Action<IRedisSagaRepositoryConfigurator> configure)
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
            configurator.RedisRepository(r => _configure?.Invoke(r));
        }


        interface IProxy
        {
            public void Configure<T>(T provider)
                where T : RedisSagaRepositoryRegistrationProvider;
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
                where T : RedisSagaRepositoryRegistrationProvider
            {
                provider.Configure(_configurator);
            }
        }
    }
}
