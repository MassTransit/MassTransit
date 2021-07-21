namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class ActivityImpl :
        Activity
    {
        protected ActivityImpl()
        {
        }

        public ActivityImpl(string name, Uri address, IDictionary<string, object> arguments)
        {
            Name = name;
            Address = address;
            Arguments = arguments;
        }

        public string Name { get; private set; }
        public Uri Address { get; private set; }
        public IDictionary<string, object> Arguments { get; private set; }
    }
}
