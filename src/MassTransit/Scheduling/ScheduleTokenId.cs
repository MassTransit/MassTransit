namespace MassTransit.Scheduling
{
    public static class ScheduleTokenId
    {
        public static void UseTokenId<T>(ScheduleTokenIdCache<T>.TokenIdSelector tokenIdSelector)
            where T : class
        {
            ScheduleTokenIdCache<T>.UseTokenId(tokenIdSelector);
        }
    }
}
