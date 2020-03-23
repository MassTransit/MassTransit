namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Threading;
    using Hangfire;


    public interface IHangfireComponentResolver
    {
        IBackgroundJobClient BackgroundJobClient { get; }
        IRecurringJobManager RecurringJobManager { get; }
        ITimeZoneResolver TimeZoneResolver { get; }
    }


    class DefaultHangfireComponentResolver : IHangfireComponentResolver
    {
        readonly Lazy<IBackgroundJobClient> _backgroundJobClient;
        readonly Lazy<IRecurringJobManager> _recurringJobManager;
        readonly Lazy<ITimeZoneResolver> _timeZoneResolver;
        public static readonly Lazy<IHangfireComponentResolver> Instance;

        static DefaultHangfireComponentResolver()
        {
            Instance = new Lazy<IHangfireComponentResolver>(() => new DefaultHangfireComponentResolver());
        }

        DefaultHangfireComponentResolver()
        {
            _backgroundJobClient = new Lazy<IBackgroundJobClient>(() => new BackgroundJobClient(), LazyThreadSafetyMode.PublicationOnly);
            _recurringJobManager = new Lazy<IRecurringJobManager>(() => new RecurringJobManager(), LazyThreadSafetyMode.PublicationOnly);
            _timeZoneResolver = new Lazy<ITimeZoneResolver>(() => new DefaultTimeZoneResolver(), LazyThreadSafetyMode.PublicationOnly);
        }

        public IBackgroundJobClient BackgroundJobClient => _backgroundJobClient.Value;
        public IRecurringJobManager RecurringJobManager => _recurringJobManager.Value;
        public ITimeZoneResolver TimeZoneResolver => _timeZoneResolver.Value;
    }
}
