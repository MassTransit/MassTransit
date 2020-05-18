namespace MassTransit.HangfireIntegration
{
    public static class HangfireMessageHeaders
    {
        public const string Sent = "MT-Hangfire-Sent";
        public const string Scheduled = "MT-Hangfire-Scheduled";
        public const string TriggerKey = "MT-Hangfire-TriggerKey";
    }
}
