namespace MassTransit.Scoping
{
    using GreenPipes;


    public interface IMessageScopeProvider :
        IProbeSite
    {
        IMessageScopeContext<T> GetScope<T>(ConsumeContext<T> context)
            where T : class;
    }
}
