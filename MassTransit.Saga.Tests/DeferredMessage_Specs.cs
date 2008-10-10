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
namespace MassTransit.Saga.Tests
{
    using System;
    using System.Collections.Generic;
    using Magnum.Infrastructure;
    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;
    using NUnit.Framework;
    using ServiceBus.Services.MessageDeferral;

    [TestFixture]
    public class DeferredMessage_Specs
    {
        private ISessionFactory _sessionFactory;
        private const string _connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";

        [Serializable]
        public class HugeMessage
        {
            private readonly List<string> _strings = new List<string>();

            public HugeMessage()
            {
                for (int i = 0; i < 100; i++)
                {
                    _strings.Add("This is a really long string and will likely cause the world to end on " + DateTime.Now.ToLongDateString());
                }
            }

            public string this[int index]
            {
                get { return _strings[index]; }
            }

            public int Count
            {
                get { return _strings.Count; }
            }
        }

        [Test]
        public void FIRST_TEST_NAME()
        {
            Configuration cfg = new Configuration();

            cfg.SetProperty("dialect", "NHibernate.Dialect.MsSql2005Dialect");

        	cfg.AddAssembly("MassTransit.Infrastructure");

            new SchemaExport(cfg, cfg.Properties).Create(true, false);
        }

        [Test]
        public void I_should_be_able_to_save_and_load()
        {
            Configuration cfg = new Configuration();

            cfg.SetProperty("dialect", "NHibernate.Dialect.MsSql2005Dialect");
            cfg.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            cfg.SetProperty("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
            cfg.SetProperty("connection.connection_string", _connectionString);

			cfg.AddAssembly("MassTransit.Infrastructure");

            _sessionFactory = cfg.BuildSessionFactory();

            HugeMessage hm = new HugeMessage();

            DeferredMessage dm = new DeferredMessage(Guid.NewGuid(), DateTime.Now + TimeSpan.FromSeconds(30), hm);

            using (ISession session = _sessionFactory.OpenSession())
            {
                session.Save(dm);
                session.Flush();
            }

            using (ISession session = _sessionFactory.OpenSession())
            {
                DeferredMessage loaded = session.Get<DeferredMessage>(dm.Id);

                Assert.AreEqual(loaded.Id, dm.Id);

                HugeMessage hm2 = loaded.GetMessage<HugeMessage>();
                for (int i = 0; i < hm.Count; i++)
                {
                    Assert.AreEqual(hm[i], hm2[i]);
                }
            }
        }
    }
}