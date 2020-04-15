namespace MassTransit.Scoping
{
    using GreenPipes;


    public interface IPublishScopeProvider :
        IProbeSite
    {
        IPublishScopeContext<T> GetScope<T>(PublishContext<T> context)
            where T : class;
    }
}
