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

        static void Integrate(DbIntegrationConfigurationProperties db, string connectionString)
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
