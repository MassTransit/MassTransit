// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.NHibernateIntegration.Tests
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using NHibernate;
    using NHibernate.Cache;
    using NHibernate.Cfg;
    using NHibernate.Cfg.Loquacious;
    using NHibernate.Dialect;
    using NHibernate.Tool.hbm2ddl;


    /// <summary>
    /// Creates a session factory that works with SQLite, by default in memory, for testing purposes
    /// </summary>
    public class SQLiteSessionFactoryProvider :
        NHibernateSessionFactoryProvider,
        IDisposable
    {
        const string InMemoryConnectionString = "Data Source=:memory:;Version=3;New=True;Pooling=True;Max Pool Size=1;";
        bool _disposed;
        ISessionFactory _innerSessionFactory;
        SQLiteConnection _openConnection;
        SingleConnectionSessionFactory _sessionFactory;

        public SQLiteSessionFactoryProvider(string connectionString, params Type[] mappedTypes)
            : base(mappedTypes, x => Integrate(x, connectionString, false))
        {
        }

        public SQLiteSessionFactoryProvider(params Type[] mappedTypes)
            : this(false, mappedTypes)
        {
        }

        public SQLiteSessionFactoryProvider(bool logToConsole, params Type[] mappedTypes)
            : base(mappedTypes, x => Integrate(x, null, logToConsole))
        {
            Configuration.SetProperty(NHibernate.Cfg.Environment.UseSecondLevelCache, "true");
            Configuration.SetProperty(NHibernate.Cfg.Environment.UseQueryCache, "true");
            Configuration.SetProperty(NHibernate.Cfg.Environment.CacheProvider, typeof(HashtableCacheProvider).AssemblyQualifiedName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SQLiteSessionFactoryProvider()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_openConnection != null)
                {
                    _openConnection.Close();
                    _openConnection.Dispose();
                }
            }

            _disposed = true;
        }

        public override ISessionFactory GetSessionFactory()
        {
            string connectionString = Configuration.Properties[NHibernate.Cfg.Environment.ConnectionString];
            _openConnection = new SQLiteConnection(connectionString);
            _openConnection.Open();

            BuildSchema(Configuration, _openConnection);

            _innerSessionFactory = base.GetSessionFactory();

            if (connectionString == InMemoryConnectionString)
            {
                _innerSessionFactory.WithOptions().Connection(_openConnection).OpenSession();

                _sessionFactory = new SingleConnectionSessionFactory(_innerSessionFactory, _openConnection);

                return _sessionFactory;
            }

            return _innerSessionFactory;
        }

        static void BuildSchema(Configuration config, DbConnection connection)
        {
            new SchemaExport(config).Execute(true, true, false, connection, null);
        }

        static void Integrate(IDbIntegrationConfigurationProperties db, string connectionString, bool logToConsole)
        {
            db.Dialect<SQLiteDialect>(); //This is a custom dialect

            db.ConnectionString = connectionString ?? InMemoryConnectionString;
            db.BatchSize = 100;
            db.IsolationLevel = IsolationLevel.Serializable;
            db.LogSqlInConsole = logToConsole;
            db.LogFormattedSql = logToConsole;
            db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;

            // Do not use this property with real DB as it will modify schema
            db.SchemaAction = SchemaAutoAction.Update;

            //Disable comments until this issue is resolved
            // https://groups.google.com/forum/?fromgroups=#!topic/nhusers/xJ675yG2uhY
            //properties.AutoCommentSql = true;
        }
    }
}
