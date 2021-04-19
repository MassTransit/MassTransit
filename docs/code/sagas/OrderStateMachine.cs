namespace PersistedSaga
{
    using Automatonymous;

    public class OrderStateMachine :
        MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
        }
    }
}
