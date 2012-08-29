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
namespace MassTransit.Transports.Msmq.Tests.TestFixtures
{
    using System.Data;
    using NHibernate;
    using NHibernate.Dialect;
    using NHibernateIntegration;
    using NHibernateIntegration.Tests.Sagas;
    using NUnit.Framework;

    [TestFixture, Category("Integration")]
    public class MsmqConcurrentSagaTestFixtureBase :
        MsmqTransactionalEndpointTestFixture
    {
        protected ISessionFactory SessionFactory { get; private set; }

        protected override void EstablishContext()
        {
            var provider = new NHibernateSessionFactoryProvider(new[]
                {
                    typeof(ConcurrentSagaMap), typeof(ConcurrentLegacySagaMap)
                }, x =>
                    {
                        x.Dialect<MsSql2008Dialect>();
                        x.BatchSize = 100;
                        x.ConnectionString = "Server=(local);initial catalog=test;Trusted_Connection=yes";
                        x.LogSqlInConsole = true;
                        x.LogFormattedSql = true;
                        x.IsolationLevel = IsolationLevel.RepeatableRead;
//                        .DefaultSchema("dbo")
                    });

            SessionFactory = provider.GetSessionFactory();

            base.EstablishContext();
        }
    }
}