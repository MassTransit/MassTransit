namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    [Serializable]
    class ActivityImpl :
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

        public string Name { get; set; }
        public Uri Address { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
    }
}
