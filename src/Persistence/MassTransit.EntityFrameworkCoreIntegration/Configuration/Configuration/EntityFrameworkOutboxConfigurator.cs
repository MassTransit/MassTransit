#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Data;
    using EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Middleware;


    public class EntityFrameworkOutboxConfigurator<TDbContext> :
        IEntityFrameworkOutboxConfigurator
        where TDbContext : DbContext
    {
        readonly IBusRegistrationConfigurator _configurator;
        IsolationLevel _isolationLevel;
        ILockStatementProvider _lockStatementProvider;

        public EntityFrameworkOutboxConfigurator(IBusRegistrationConfigurator configurator)
        {
            _configurator = configurator;

            _lockStatementProvider = new SqlServerLockStatementProvider();
            _isolationLevel = IsolationLevel.Serializable;

            configurator.TryAddScoped<IOutboxContextFactory<TDbContext>, EntityFrameworkOutboxContextFactory<TDbContext>>();
            configurator.AddOptions<EntityFrameworkOutboxOptions>().Configure(options =>
            {
                options.IsolationLevel = _isolationLevel;
                options.LockStatementProvider = _lockStatementProvider;
            });

            configurator.AddHostedService<InboxCleanupService<TDbContext>>();
            configurator.AddOptions<InboxCleanupServiceOptions>().Configure(options =>
            {
                options.DuplicateDetectionWindow = DuplicateDetectionWindow;
                options.QueryMessageLimit = QueryMessageLimit;
                options.QueryDelay = QueryDelay;
                options.QueryTimeout = QueryTimeout;
            });
        }

        public TimeSpan DuplicateDetectionWindow { get; set; } = TimeSpan.FromMinutes(30);

        public IsolationLevel IsolationLevel
        {
            set => _isolationLevel = value;
        }

        public ILockStatementProvider LockStatementProvider
        {
            set => _lockStatementProvider = value ?? throw new ConfigurationException("LockStatementProvider must not be null");
        }

        public TimeSpan QueryDelay { get; set; } = TimeSpan.FromSeconds(10);

        public int QueryMessageLimit { get; set; } = 100;

        public TimeSpan QueryTimeout { get; set; } = TimeSpan.FromSeconds(30);

        public virtual void UseBusOutbox(Action<IEntityFrameworkBusOutboxConfigurator>? configure = null)
        {
            var busOutboxConfigurator = new EntityFrameworkBusOutboxConfigurator<TDbContext>(_configurator, this);

            busOutboxConfigurator.Configure(configure);
        }
    }
}
