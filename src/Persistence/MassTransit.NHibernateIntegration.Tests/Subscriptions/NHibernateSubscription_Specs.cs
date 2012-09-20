// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.NHibernateIntegration.Tests.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BusConfigurators;
    using MassTransit.Subscriptions.Coordinator;
    using MassTransit.Tests.TextFixtures;
    using NHibernate;
    using NHibernateIntegration.Subscriptions;
    using NUnit.Framework;
    using Testing;

    [TestFixture]
    public class When_a_bus_has_peer_subscriptions :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_recover_the_subscriptions_after_restarting()
        {
            Assert.IsTrue(LocalBus.HasSubscription<Hello>().Any(), "Initial subscription not found");

            RemoteBus.Dispose();
            RemoteBus = null;

            LocalBus.Dispose();
            LocalBus = null;

            // now we need to reload the local bus
            LocalBus = ServiceBusFactory.New(ConfigureLocalBus);

            Assert.IsTrue(LocalBus.HasSubscription<Hello>().Any(), "Subscription not found after restart");
        }

        ISessionFactory _sessionFactory;
        SqlLiteSessionFactoryProvider _provider;

        protected override void EstablishContext()
        {
            _provider = new SqlLiteSessionFactoryProvider(typeof(PersistentSubscriptionMap));
            _sessionFactory = _provider.GetSessionFactory();

            base.EstablishContext();
        }

        protected override void TeardownContext()
        {
            base.TeardownContext();

            DumpSubscriptionContent();

            if (_sessionFactory != null)
                _sessionFactory.Dispose();

            if (_provider != null)
                _provider.Dispose();
        }

        void DumpSubscriptionContent()
        {
            IList<PersistentSubscription> subscriptions;

            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                subscriptions = session.QueryOver<PersistentSubscription>()
                    .OrderBy(x => x.PeerUri).Asc
                    .ThenBy(x => x.MessageName).Asc
                    .ThenBy(x => x.EndpointUri).Asc
                    .ReadOnly()
                    .List();

                transaction.Commit();
            }

            foreach (PersistentSubscription subscription in subscriptions)
            {
                Console.WriteLine(subscription);
            }
        }


        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.UseNHibernateSubscriptionStorage(_sessionFactory);

            base.ConfigureLocalBus(configurator);
        }


        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.UseNHibernateSubscriptionStorage(_sessionFactory);

            configurator.Subscribe(x => { x.Handler<Hello>(message => { }); });
        }

        interface Hello
        {
        }
    }
}