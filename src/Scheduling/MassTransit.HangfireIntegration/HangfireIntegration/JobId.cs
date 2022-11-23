namespace MassTransit.HangfireIntegration
{
    internal static class JobKey
    {
        public static string Create(string scheduleId, string? scheduleGroup)
        {
            return string.IsNullOrEmpty(scheduleGroup) ? scheduleId : $"{scheduleId}-{scheduleGroup}";
        }
    }
}
