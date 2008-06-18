namespace MassTransit.ServiceBus.Subscriptions
{
    using System;
    using log4net;
    using Messages;

    public class CancelUpdatesHandler :
        Consumes<CancelSubscriptionUpdates>.All
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(CancelUpdatesHandler));
        private readonly FollowerRepository _followers;


        public CancelUpdatesHandler(FollowerRepository followers)
        {
            _followers = followers;
        }

        public void Consume(CancelSubscriptionUpdates message)
        {
            try
            {
                _followers.RemoveFollower(message.RequestingUri);
            }
            catch (Exception ex)
            {
                _log.Error("Exception handling CancelSbuscriptionUpdates", ex);
            }

        }
    }
}