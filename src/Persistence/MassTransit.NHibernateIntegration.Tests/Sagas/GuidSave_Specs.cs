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
namespace MassTransit.NHibernateIntegration.Tests.Sagas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Magnum.TestFramework;
    using NHibernate;
    using NUnit.Framework;


    [TestFixture, Category("Integration")]
    public class Saving_correlation_id_values_to_sql_server_for_a_clustered_index

    {
        [SetUp]
        public void Setup()
        {
            var provider =
                new SqlServerSessionFactoryProvider(
                    "Application Name=MassTransit.Tests;Connect Timeout=30;Connection Lifetime=300;Database=Hydro;Server=.;Integrated Security=SSPI;",
                    new[]
                        {
                            typeof(ConcurrentSagaMap)
                        });

            _sessionFactory = provider.GetSessionFactory();

            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                session.Delete("from ConcurrentSaga cs");
                session.Flush();
                tx.Commit();
            }
        }

        [TearDown]
        public void Teardown()
        {
            if (_sessionFactory != null)
                _sessionFactory.Dispose();
        }

        [Test, Integration]
        public void Should_store_them_in_order()
        {
            var ids = new List<Guid>(Enumerable.Repeat(1, 100).Select(x =>
                {
                    Thread.Sleep(10);
                    return NewId.NextGuid();
                }));

            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                foreach (Guid id in ids)
                    session.Save(new ConcurrentSaga(id));

                tx.Commit();
            }

            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                IList<ConcurrentSaga> results = session.QueryOver<ConcurrentSaga>()
                    .OrderBy(x => x.CorrelationId).Asc
                    .List();

                tx.Commit();

//                var merged = results.Select(x => x.CorrelationId).Zip(ids, (Left, Right) => new {Left, Right});
//
//                foreach (var items in merged)
//                    Console.WriteLine("{0} {1}", items.Left, items.Right);


                Assert.IsTrue(ids.SequenceEqual(results.Select(x => x.CorrelationId)));
            }
        }

        ISessionFactory _sessionFactory;
    }
}