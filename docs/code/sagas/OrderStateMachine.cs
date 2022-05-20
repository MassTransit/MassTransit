namespace PersistedSaga
{
    using MassTransit;

    public class OrderStateMachine :
        MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
        }
    }
}
