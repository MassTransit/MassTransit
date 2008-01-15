namespace MassTransit.ServiceBus.Subscriptions
{
    using System;

    [Serializable]
    public class Subscription
    {
        private Uri _address;
        private string _messageName;


        public Subscription(Uri address, string messageName)
        {
            _address = address;
            _messageName = messageName;
        }


        public Uri Address
        {
            get { return _address; }
        }

        public string MessageName
        {
            get { return _messageName; }
        }
    }
}