namespace MassTransit
{
    using System;
    using Hangfire;
    using HangfireIntegration;


    public class HangfireSchedulerOptions
    {
        /// <summary>
        /// Use to retrieve components for Hangfire, defaults to <see cref="DefaultHangfireComponentResolver"/> and <see cref="JobStorage" />.
        /// </summary>
        public IHangfireComponentResolver ComponentResolver { get; set; } = DefaultHangfireComponentResolver.Instance;

        /// <summary>
        /// The queue name for the quartz service, defaults to "hangfire".
        /// </summary>
        public string QueueName { get; set; } = "hangfire";

        /// <summary>
        /// Server configurator
        /// </summary>
        public Action<BackgroundJobServerOptions>? ConfigureServer { get; set; }
    }
}
