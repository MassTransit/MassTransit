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
namespace MassTransit.NHibernateIntegration
{
    using System;
    using System.Data;
    using NHibernate.Cfg;
    using NHibernate.Cfg.Loquacious;
    using NHibernate.Dialect;


    public class SqlServerSessionFactoryProvider :
        NHibernateSessionFactoryProvider
    {
        public SqlServerSessionFactoryProvider(string connectionString, params Type[] mapTypes)
            : base(mapTypes, x => Integrate(x, connectionString))
        {
        }

        static void Integrate(IDbIntegrationConfigurationProperties db, string connectionString)
        {
            db.Dialect<MsSql2008Dialect>();
            db.ConnectionString = connectionString;
            db.BatchSize = 100;
            db.IsolationLevel = IsolationLevel.RepeatableRead;
            db.LogSqlInConsole = true;
            db.LogFormattedSql = true;
            db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
            db.SchemaAction = SchemaAutoAction.Update;
        }
    }
}