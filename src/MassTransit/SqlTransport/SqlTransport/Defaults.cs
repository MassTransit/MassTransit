namespace MassTransit.SqlTransport
{
    using System;
    using System.ComponentModel;


    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Defaults
    {
        public static TimeSpan LockDuration { get; set; } = TimeSpan.FromMinutes(5);
        public static TimeSpan DefaultMessageTimeToLive { get; set; } = TimeSpan.FromDays(365 + 1);
        public static TimeSpan ErrorQueueTimeToLive { get; set; } = TimeSpan.FromDays(14);

        public static TimeSpan AutoDeleteOnIdle { get; set; } = TimeSpan.FromDays(427);
        public static TimeSpan TemporaryAutoDeleteOnIdle { get; set; } = TimeSpan.FromMinutes(5);
        public static TimeSpan MaxAutoRenewDuration { get; set; } = TimeSpan.FromMinutes(5);

        public static TimeSpan SessionIdleTimeout { get; set; } = TimeSpan.FromSeconds(10);
        public static TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromMilliseconds(100);
    }
}
