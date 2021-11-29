namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Collections.Generic;
    using Hangfire;
    using Hangfire.Common;
    using Hangfire.Server;
    using Microsoft.Extensions.DependencyInjection;


    public class BusRegistrationContextComponentResolver :
        IHangfireComponentResolver
    {
        readonly Lazy<IBackgroundJobClient> _backgroundJobClient;
        readonly Lazy<IEnumerable<IBackgroundProcess>> _backgroundProcesses;
        readonly Lazy<IJobFilterProvider> _jobFilterProvider;
        readonly Lazy<JobStorage> _jobStorage;
        readonly Lazy<IRecurringJobManager> _recurringJobManager;
        readonly Lazy<ITimeZoneResolver> _timeZoneResolver;

        public BusRegistrationContextComponentResolver(IServiceProvider provider, IHangfireComponentResolver resolver)
        {
            _backgroundJobClient = new Lazy<IBackgroundJobClient>(() => provider.GetService<IBackgroundJobClient>() ?? resolver.BackgroundJobClient);
            _recurringJobManager = new Lazy<IRecurringJobManager>(() => provider.GetService<IRecurringJobManager>() ?? resolver.RecurringJobManager);
            _timeZoneResolver = new Lazy<ITimeZoneResolver>(() => provider.GetService<ITimeZoneResolver>() ?? resolver.TimeZoneResolver);
            _jobFilterProvider = new Lazy<IJobFilterProvider>(() => provider.GetService<IJobFilterProvider>() ?? resolver.JobFilterProvider);
            _backgroundProcesses =
                new Lazy<IEnumerable<IBackgroundProcess>>(() => provider.GetService<IEnumerable<IBackgroundProcess>>() ?? resolver.BackgroundProcesses);
            _jobStorage = new Lazy<JobStorage>(() => provider.GetService<JobStorage>() ?? resolver.JobStorage);
        }

        public IBackgroundJobClient BackgroundJobClient => _backgroundJobClient.Value;
        public IRecurringJobManager RecurringJobManager => _recurringJobManager.Value;
        public ITimeZoneResolver TimeZoneResolver => _timeZoneResolver.Value;
        public IJobFilterProvider JobFilterProvider => _jobFilterProvider.Value;
        public IEnumerable<IBackgroundProcess> BackgroundProcesses => _backgroundProcesses.Value;
        public JobStorage JobStorage => _jobStorage.Value;
    }
}
