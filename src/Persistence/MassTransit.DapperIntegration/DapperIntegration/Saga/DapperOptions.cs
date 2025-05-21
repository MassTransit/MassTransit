#nullable enable
using System;

namespace MassTransit.DapperIntegration.Saga
{
    using System.Data;
    using System.Data.Common;
    using Configuration;
    using SqlBuilders;


    public class DapperOptions<TSaga>
        where TSaga : class, ISaga
    {
        public string? ConnectionString { get; set; }
        public string? TableName { get; set; }
        public string? IdColumnName { get; set; }
        public IsolationLevel? IsolationLevel { get; set; }
        public DatabaseProviders Provider { get; set; }

        // use public fields to prevent binding attempts from the configuration
        public Func<IServiceProvider, ISagaSqlFormatter<TSaga>>? SqlBuilderProvider;
        public Func<IServiceProvider, DatabaseContextFactory<TSaga>>? ContextFactoryProvider;
        public Func<IServiceProvider, DbConnection>? DbConnectionProvider;

        [Obsolete("Use ContextFactoryProvider instead", true)]
        public DatabaseContextFactory<TSaga>? ContextFactory;
    }
}
