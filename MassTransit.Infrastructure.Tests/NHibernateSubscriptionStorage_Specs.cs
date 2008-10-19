// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Infrastructure.Tests
{
    using System;
    using NHibernate;
    using NHibernate.Cfg;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Repositories;
    using ServiceBus.Subscriptions;

    [TestFixture]
    public class When_saving_a_subscription_to_the_database
    {
        [SetUp]
        public void Setup()
        {
            _dbQuery = new DbQuery(_connectionString);

            _dbQuery.ExecuteCommand("DELETE FROM bus.Subscriptions");

            Configuration cfg = new Configuration();

            cfg.SetProperty("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            cfg.SetProperty("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
            cfg.SetProperty("hibernate.connection.connection_string", _connectionString);
            cfg.SetProperty("hibernate.dialect", "NHibernate.Dialect.MsSql2005Dialect");

            cfg.AddAssembly("MassTransit.SubscriptionStorage");

            _sessionFactory = cfg.BuildSessionFactory();

            _subscriptionRepository = new PersistantSubscriptionRepository(_sessionFactory);
        }

        [Test]
        public void Only_one_entry_for_the_message_should_exist_in_the_table()
        {
            Subscription subscription = new Subscription("a", new Uri("msmq://localhost/test_queue"));

            _subscriptionRepository.Save(subscription);
            _subscriptionRepository.Save(subscription);
            _subscriptionRepository.Save(subscription);

            Assert.That(_dbQuery.ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions"), Is.EqualTo(1), "Subscription count didn't match");
        }

        [Test]
        public void Removed_entries_should_be_marked_as_not_active()
        {
            _subscriptionRepository.Save(new Subscription("a", new Uri("msmq://localhost/test_client")));

            Assert.That(_dbQuery.ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions"), Is.EqualTo(1), "Subscription count didn't match");

            _subscriptionRepository.Remove(new Subscription("a", new Uri("msmq://localhost/test_client")));

            Assert.That(_dbQuery.ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions WHERE Message='a'"), Is.EqualTo(1), "The subscription did not exist");
            Assert.That(_dbQuery.ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions WHERE Message='a' AND IsActive = 0"), Is.EqualTo(1), "The subscription was not marked as inactive");
        }

        [Test]
        public void The_case_of_type_names_and_uri_paths_should_not_matter()
        {
            _subscriptionRepository.Save(new Subscription("b", new Uri("msmq://localhost/test_client")));
            _subscriptionRepository.Save(new Subscription("b", new Uri("msmq://localhost/test_client")));

            //casing of message shouldn't matter
            _subscriptionRepository.Save(new Subscription("B", new Uri("msmq://localhost/test_client")));

            //casing of uri shouldn't matter
            _subscriptionRepository.Save(new Subscription("b", new Uri("msmq://Localhost/test_client")));

            //spaces on message name should be ignored
            _subscriptionRepository.Save(new Subscription("b ", new Uri("msmq://localhost/test_client")));

            //spaces on uri should be ignored
            _subscriptionRepository.Save(new Subscription("b", new Uri("msmq://localhost/test_client ")));

            Assert.That(_dbQuery.ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions"), Is.EqualTo(1), "Subscription count didn't match");
        }

        [Test]
        public void The_subscription_should_exist_in_the_table()
        {
            Subscription subscription = new Subscription("a", new Uri("msmq://localhost/test_queue"));

            _subscriptionRepository.Save(subscription);

            Assert.That(_dbQuery.ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions"), Is.EqualTo(1), "Subscription count didn't match");
        }

        private readonly string _connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";
        private ISessionFactory _sessionFactory;
        private ISubscriptionRepository _subscriptionRepository;
        private DbQuery _dbQuery;
    }
}