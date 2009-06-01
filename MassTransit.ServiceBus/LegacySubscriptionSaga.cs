namespace MassTransit.ServiceBus
{
    using System;
    using Magnum.StateMachine;
    using Saga;
    using Services.Subscriptions.Messages;
    using AddSubscription=MassTransit.ServiceBus.Subscriptions.Messages.AddSubscription;
    using RemoveSubscription=MassTransit.ServiceBus.Subscriptions.Messages.RemoveSubscription;

    public class LegacySubscriptionSaga :
        SagaStateMachine<LegacySubscriptionSaga>,
        ISaga
    {
        static LegacySubscriptionSaga()
        {
            Define(()=>
            {
                Initially(
                    When(SubscriptionAdded)
                    .Then((saga,message)=>
                    {
                        //option 1
                        var si = new SubscriptionInformation(Guid.NewGuid(), 1, message.Subscription.MessageName, message.Subscription.CorrelationId, message.Subscription.EndpointUri);
                        var msg = new MassTransit.Services.Subscriptions.Messages.AddSubscription(si);
                        saga.Bus.Endpoint.Send(msg);
                    })
                    .TransitionTo(Active));
                During(Active,
                    When(SubscriptionRemoved)
                    .Then((saga,message)=>
                    {
                        //option 1
                        var si = new SubscriptionInformation(Guid.NewGuid(), 1, message.Subscription.MessageName, message.Subscription.CorrelationId, message.Subscription.EndpointUri);
                        var msg = new MassTransit.Services.Subscriptions.Messages.RemoveSubscription(si);
                        saga.Bus.Endpoint.Send(msg);
                    })
                    .Complete());
            });
        }

        public Guid CorrelationId { get; set; }
        public IServiceBus Bus { get; set; }

        public static State Initial { get; set; }
        public static State Active { get; set; }
        public static State Completed { get; set; }

        public static Event<AddSubscription> SubscriptionAdded { get; set; }
        public static Event<RemoveSubscription> SubscriptionRemoved { get; set; }

        public SubscriptionInformation SubscriptionInfo { get; set; }
    }
}