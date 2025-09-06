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
        bool _registerInboxCleanupService;

        public EntityFrameworkOutboxConfigurator(IBusRegistrationConfigurator configurator)
        {
            _configurator = configurator;

            _lockStatementProvider = new SqlServerLockStatementProvider();
            _isolationLevel = IsolationLevel.RepeatableRead;
            _registerInboxCleanupService = true;
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

        public void DisableInboxCleanupService()
        {
            _registerInboxCleanupService = false;
        }

        public virtual void UseBusOutbox(Action<IEntityFrameworkBusOutboxConfigurator>? configure = null)
        {
            var busOutboxConfigurator = new EntityFrameworkBusOutboxConfigurator<TDbContext>(_configurator, this);

            busOutboxConfigurator.Configure(configure);
        }

        public virtual void Configure(Action<IEntityFrameworkOutboxConfigurator>? configure)
        {
            configure?.Invoke(this);

            _configurator.TryAddScoped<IOutboxContextFactory<TDbContext>, EntityFrameworkOutboxContextFactory<TDbContext>>();
            _configurator.AddOptions<EntityFrameworkOutboxOptions<TDbContext>>().Configure(options =>
            {
                options.IsolationLevel = _isolationLevel;
                options.LockStatementProvider = _lockStatementProvider;
            });

            if (_registerInboxCleanupService)
            {
                _configurator.AddHostedService<InboxCleanupService<TDbContext>>();
                _configurator.AddOptions<InboxCleanupServiceOptions<TDbContext>>().Configure(options =>
                {
                    options.DuplicateDetectionWindow = DuplicateDetectionWindow;
                    options.QueryMessageLimit = QueryMessageLimit;
                    options.QueryDelay = QueryDelay;
                    options.QueryTimeout = QueryTimeout;
                });
            }
        }
    }
}
