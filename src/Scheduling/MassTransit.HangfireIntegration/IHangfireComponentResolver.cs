namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Hangfire;
    using Hangfire.Common;
    using Hangfire.Server;


    public interface IHangfireComponentResolver
    {
        IBackgroundJobClient BackgroundJobClient { get; }
        IRecurringJobManager RecurringJobManager { get; }
        ITimeZoneResolver TimeZoneResolver { get; }
        IJobFilterProvider JobFilterProvider { get; }
        IEnumerable<IBackgroundProcess> BackgroundProcesses { get; }
        JobStorage JobStorage { get; }
    }


    class DefaultHangfireComponentResolver :
        IHangfireComponentResolver
    {
        readonly Lazy<IBackgroundJobClient> _backgroundJobClient;
        readonly Lazy<IRecurringJobManager> _recurringJobManager;
        readonly Lazy<ITimeZoneResolver> _timeZoneResolver;
        readonly Lazy<JobStorage> _jobStorage;
        readonly Lazy<IJobFilterProvider> _jobFilterProvider;
        static readonly Lazy<IHangfireComponentResolver> Cached;
        public static IHangfireComponentResolver Instance => Cached.Value;

        static DefaultHangfireComponentResolver()
        {
            Cached = new Lazy<IHangfireComponentResolver>(() => new DefaultHangfireComponentResolver());
        }

        DefaultHangfireComponentResolver()
        {
            _jobStorage = new Lazy<JobStorage>(() => JobStorage.Current, LazyThreadSafetyMode.PublicationOnly);
            _jobFilterProvider = new Lazy<IJobFilterProvider>(() => JobFilterProviders.Providers, LazyThreadSafetyMode.PublicationOnly);
            _backgroundJobClient = new Lazy<IBackgroundJobClient>(
                () => new BackgroundJobClient(JobStorage, JobFilterProvider), LazyThreadSafetyMode.PublicationOnly);
            _timeZoneResolver = new Lazy<ITimeZoneResolver>(() => new DefaultTimeZoneResolver(), LazyThreadSafetyMode.PublicationOnly);
            _recurringJobManager = new Lazy<IRecurringJobManager>(
                () => new RecurringJobManager(JobStorage, JobFilterProvider, TimeZoneResolver), LazyThreadSafetyMode.PublicationOnly);
            BackgroundProcesses = Enumerable.Empty<IBackgroundProcess>();
        }

        public IEnumerable<IBackgroundProcess> BackgroundProcesses { get; }
        public IBackgroundJobClient BackgroundJobClient => _backgroundJobClient.Value;
        public IRecurringJobManager RecurringJobManager => _recurringJobManager.Value;
        public ITimeZoneResolver TimeZoneResolver => _timeZoneResolver.Value;
        public IJobFilterProvider JobFilterProvider => _jobFilterProvider.Value;
        public JobStorage JobStorage => _jobStorage.Value;
    }
}
