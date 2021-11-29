namespace MassTransit.Courier.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Contracts;


    [Serializable]
    public class ActivityImpl :
        Activity
    {
        public ActivityImpl()
        {
        }

        public ActivityImpl(string name, Uri address, IDictionary<string, object> arguments)
        {
            Name = name;
            Address = address;
            Arguments = arguments;
        }

        public ActivityImpl(Activity activity)
        {
            if (string.IsNullOrEmpty(activity.Name))
                throw new SerializationException("An Activity Name is required");
            if (activity.Address == null)
                throw new SerializationException("An Activity ExecuteAddress is required");

            Name = activity.Name;
            Address = activity.Address;
            Arguments = activity.Arguments ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; set; }
        public Uri Address { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
    }
}
