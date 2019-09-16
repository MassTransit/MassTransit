namespace MassTransit.Context
{
    using System;


    public interface StartedActivityContext
    {
        void AddTag(string key, string value);
        void AddTag(string key, Guid? value);
        void AddTag(string key, Uri value);
        void AddBaggage(string key, string value);
        void AddBaggage(string key, Guid? value);

        void SetParentId(string parentId);
    }
}
