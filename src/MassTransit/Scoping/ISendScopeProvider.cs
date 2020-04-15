namespace MassTransit.Scoping
{
    using GreenPipes;


    public interface ISendScopeProvider :
        IProbeSite
    {
        ISendScopeContext<T> GetScope<T>(SendContext<T> context)
            where T : class;
    }
}
