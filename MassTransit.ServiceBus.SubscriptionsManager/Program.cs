/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System;
    using System.ServiceProcess;
    using NHibernate;
    using NHibernate.Cfg;
    using Subscriptions;

    public class Program : ServiceBase
    {
        private SubscriptionService _subscriptionService;
        private IEndpoint _endpoint;
        private ISessionFactory _sessionFactory;
        private ISubscriptionStorage _subscriptionCache;

        protected void Initialize(string connectionString, string wellKnownSubscriptionUri)
        {
			//TODO
            //_endpoint = new MessageQueueEndpoint(wellKnownSubscriptionUri);
            _sessionFactory = GetNHibernateSessionFactory(connectionString);

            _subscriptionCache = new LocalSubscriptionCache();


            _subscriptionService = new SubscriptionService(
                new ServiceBus(_endpoint, _subscriptionCache),
                _subscriptionCache,
                new PersistantSubscriptionStorage(_sessionFactory)
);
        }


        protected override void OnStart(string[] args)
        {
            if (args.Length != 1)
                throw new ArgumentException("There needs to be one parameter that is a connection string");

            base.OnStart(args);

            Initialize(args[0], args[1]);
            
            _subscriptionService.Start(args);
        }

        protected override void OnStop()
        {
            _subscriptionService.Stop();

            base.OnStop();
        }

        protected ISessionFactory GetNHibernateSessionFactory(string connectionString)
        {
            Configuration cfg = new Configuration();

            cfg.SetProperty("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            cfg.SetProperty("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
            cfg.SetProperty("hibernate.connection.connection_string", connectionString);
            cfg.SetProperty("hibernate.dialect", "NHibernate.Dialect.MsSql2005Dialect");

            cfg.AddAssembly("MassTransit.ServiceBus.SubscriptionsManager");

            return cfg.BuildSessionFactory();
        }
    }
}