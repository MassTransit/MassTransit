namespace MassTransit.ServiceBus
{
    using System;
    using MassTransit.ServiceBus.Subscriptions.Messages;

    public class SubscriptionChangedEventArgs : EventArgs
    {
        private SubscriptionChange _change;


        public SubscriptionChangedEventArgs(SubscriptionChange change)
        {
            _change = change;
        }

        public SubscriptionChange Change
        {
            get { return _change; }
        }
    }
}