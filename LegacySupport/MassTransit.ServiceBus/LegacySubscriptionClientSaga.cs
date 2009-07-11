namespace MassTransit.ServiceBus
{
    using System;
    using Magnum.StateMachine;
    using Messages;
    using Saga;
    //old stuff
    using OldAddSubscription = MassTransit.ServiceBus.Subscriptions.Messages.AddSubscription;
    using OldRemoveSubscription = MassTransit.ServiceBus.Subscriptions.Messages.RemoveSubscription;
    using OldCacheUpdateRequest = MassTransit.ServiceBus.Subscriptions.Messages.CacheUpdateRequest;
    using OldCacheUpdateResponse = MassTransit.ServiceBus.Subscriptions.Messages.CacheUpdateResponse;
    using OldCancelSubscriptionUpdates = MassTransit.ServiceBus.Subscriptions.Messages.CancelSubscriptionUpdates;


    public class LegacySubscriptionClientSaga :
        SagaStateMachine<LegacySubscriptionClientSaga>,
        ISaga
    {
        static LegacySubscriptionClientSaga()
        {
            Define(()=>
            {
                Correlate(OldCacheUpdateRequested).By((saga,message)=> saga.DataUri == message.RequestingUri);
                Correlate(OldSubscriptionAdded).By((saga,message)=> saga.DataUri == message.Subscription.EndpointUri);
                Correlate(OldSubscriptionRemoved).By((saga,message)=> saga.DataUri == message.Subscription.EndpointUri);

                Initially(
                    When(OldCacheUpdateRequested) 
                        .Then(InitialAction)
                        .TransitionTo(Active),
                    When(OldSubscriptionAdded)
                        .Then(InitialAdd),
                    When(OldSubscriptionRemoved)
                        .Then(InitialRemoved));
                
                During(Active,
                    When(OldCancelSubscriptionUpdates)
                    .Then((saga, message) => saga.NotifyLegacySubscriptionClientRemoved())
                    .Complete());
            });
        }

        public LegacySubscriptionClientSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        protected LegacySubscriptionClientSaga()
        {
            
        }



        public static State Initial { get; set; }
        public static State Active { get; set; }
        public static State Completed { get; set; }

        public static Event<OldAddSubscription> OldSubscriptionAdded { get; set; }
        public static Event<OldRemoveSubscription> OldSubscriptionRemoved { get; set; }
        public static Event<OldCacheUpdateRequest> OldCacheUpdateRequested { get; set; }
        public static Event<OldCancelSubscriptionUpdates> OldCancelSubscriptionUpdates { get; set; }


        public virtual Uri ControlUri { get; set; }
        public virtual Uri DataUri { get; set; }

        public Guid CorrelationId { get; set; }
        public IServiceBus Bus { get; set; }

        void NotifyLegacySubscriptionClientRemoved()
        {
            var message = new LegacySubscriptionClientRemoved
            {
                CorrelationId = CorrelationId,
                ControlUri = ControlUri,
                DataUri = DataUri,
            };

            Bus.Publish(message);
        }

        void NotifyLegacySubscriptionClientAdded()
        {
            var message = new LegacySubscriptionClientAdded
            {
                ClientId = CorrelationId,
                ControlUri = ControlUri,
                DataUri = DataUri,
            };

            Bus.Publish(message);
        }

        static void InitialAction(LegacySubscriptionClientSaga saga, OldCacheUpdateRequest message)
        {
            saga.ControlUri = saga.Bus.Endpoint.Uri;
            saga.DataUri = message.RequestingUri;

            saga.NotifyLegacySubscriptionClientAdded();
        }
        static void InitialAdd(LegacySubscriptionClientSaga saga, OldAddSubscription message)
        {
            saga.ControlUri = saga.Bus.Endpoint.Uri;
            saga.DataUri = message.Subscription.EndpointUri; //TODO: is this right?

            saga.NotifyLegacySubscriptionClientAdded();
        }
        static void InitialRemoved(LegacySubscriptionClientSaga saga, OldRemoveSubscription message)
        {
            saga.ControlUri = saga.Bus.Endpoint.Uri;
            saga.DataUri = message.Subscription.EndpointUri; //TODO: is this right?

            saga.NotifyLegacySubscriptionClientAdded();
        }
    }
}