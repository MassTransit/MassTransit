namespace MassTransit.ServiceBus.Subscriptions.Messages
{
	using System;

	[Serializable]
    public class CancelSubscriptionUpdates 
    {
        private readonly Uri _RequestingUri;


        public CancelSubscriptionUpdates(Uri requstingUri)
        {
            _RequestingUri = requstingUri;
        }


        public Uri RequestingUri
        {
            get { return _RequestingUri; }
        }
    }
}