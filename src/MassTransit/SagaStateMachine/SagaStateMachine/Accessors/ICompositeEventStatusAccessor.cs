namespace MassTransit.SagaStateMachine
{
    public interface ICompositeEventStatusAccessor<in TSaga> :
        IProbeSite
    {
        CompositeEventStatus Get(TSaga instance);

        void Set(TSaga instance, CompositeEventStatus status);
    }
}
