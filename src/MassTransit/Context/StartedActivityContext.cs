namespace MassTransit.Context
{
    public interface StartedActivityContext
    {
        StartedActivity AddTag(string key, string value);
        StartedActivity AddBaggage(string key, string value);

        void SetParentId(string parentId);
    }
}
