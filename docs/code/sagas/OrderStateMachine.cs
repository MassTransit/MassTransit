namespace MongoDbSaga
{
    using System;
    using MassTransit;
    using MassTransit.Saga;
    using Automatonymous;

    public class OrderStateMachine :
        MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
        }
    }
}